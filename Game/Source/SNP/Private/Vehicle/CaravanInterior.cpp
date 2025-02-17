#include "Vehicle/CaravanInterior.h"

#include "Game/GameAPISubsystem.h"
#include "Game/Furnitures/Chest.h"
#include "Game/LevelSimulation/LevelSimulationSubsystem.h"
#include "Game/Save/SaveState.h"
#include "GameFramework/Character.h"
#include "GameFramework/CharacterMovementComponent.h"
#include "Utils/Utility.h"

ACaravanInterior::ACaravanInterior()
{
	PrimaryActorTick.bCanEverTick = false;
	bReplicates = true;
    
	RootComponent = CreateDefaultSubobject<USceneComponent>(TEXT("SceneComponent"));;
}

void ACaravanInterior::BeginPlay()
{
	Super::BeginPlay();
	if(HasAuthority())
	{
		LoadData();
	}
}

void ACaravanInterior::GetLifetimeReplicatedProps(TArray<FLifetimeProperty>& OutLifetimeProps) const
{
	Super::GetLifetimeReplicatedProps(OutLifetimeProps);
    
	// DOREPLIFETIME(ACaravanInterior, VisiblePlayers);
}

void ACaravanInterior::SaveData()
{
	FSaveDataArray SaveDataArray;
	TArray<AActor*> AttachedActors; 
	this->GetAttachedActors(AttachedActors); 
	for (auto* Attached : AttachedActors)
	{    
		ISaveable* SaveableEntity = Cast<ISaveable>(Attached);
		if(SaveableEntity == nullptr) continue;

		const TArray<uint8> SaveState = SaveableEntity->GetSaveState();
		FSaveData SaveData;
		SaveData.Base64Data = FBase64::Encode(SaveState);
		SaveDataArray.SaveData.Add(SaveData);
	}
	
	if(UGameAPISubsystem* APISubsystem = GetGameInstance()->GetSubsystem<UGameAPISubsystem>())
	{
		APISubsystem->SendSaveCommand(SaveDataArray);
	}
}

void ACaravanInterior::LoadData()
{
	UGameAPISubsystem* GameAPISubsystem = GetGameInstance()->GetSubsystem<UGameAPISubsystem>();
	GameAPISubsystem->LoadData(0, 
		[this](const FLoadedDataArray& Response, const bool Success)
		{
			if (!Success)
			{
				UE_LOG(LogTemp, Warning, TEXT("Failed to send load command to API."));
			}

			UE_LOG(LogTemp, Log, TEXT("Response struct: %s"), *Response.ToString());
			for (auto Data : Response.DataArray)
			{
				TArray<uint8> BinData = Data.GetBinaryData();
			
				int32 ExtractedId;
				FMemoryReader Reader = FMemoryReader(BinData);
				Reader << ExtractedId;

				UE_LOG(LogTemp, Log, TEXT("Extracted ID: %d"), ExtractedId);
				
				FSavedObjectKey* FoundObject = Algo::FindByPredicate(ActorsReferences,
					[ExtractedId](const FSavedObjectKey& Key)
				{
					return Key.Id == ExtractedId;
				});

				if(FoundObject == nullptr)
				{
					UE_LOG(LogTemp, Warning, TEXT("Unknown object id %d. Can't parse data"), ExtractedId);
					return;
				}
				
				if(FoundObject->Name == "Chest")
				{
					if (!(FoundObject->Type && FoundObject->Type->IsChildOf(AChest::StaticClass())))
					{
					   UE_LOG(LogTemp, Warning, TEXT("Type is not AChest"));
					   return;
					}
					
					UWorld* World = GetWorld();
					if (!GetWorld())
					{
					   UE_LOG(LogTemp, Warning, TEXT("Invalid world"));
					   return;
					}
					AChest* SpawnedChest = World->SpawnActor<AChest>(FoundObject->Type, FVector::ZeroVector, FRotator::ZeroRotator);
					if (SpawnedChest)
					{
					   UE_LOG(LogTemp, Warning, TEXT("Successfully spawned AChest: %s"), *SpawnedChest->GetName());
					}
					else
					{
					   UE_LOG(LogTemp, Error, TEXT("Failed to spawn AChest"));
					   return;
					}
					
					TArray<uint8> BinaryData = Data.GetBinaryData();
					FMemoryReader MemReader(BinaryData);
					SpawnedChest->LoadSaveState(MemReader);
					SpawnedChest->AttachToActor(this, FAttachmentTransformRules::KeepRelativeTransform);
					SpawnedChest->SetReplicates(true);
				}
			}
		});
}

// Simulated level interface implementation

void ACaravanInterior::Init(FLevelSimulationData* SimulationData)
{
	Data = SimulationData;
}

void ACaravanInterior::Simulate(float DeltaTime)
{
	// Levels are static for now
	// Simulate lightweight data related to the inside of the level 
}

void ACaravanInterior::Enter(APlayerController* PlayerController)
{
	if(!VALIDATE_PARAMS(PlayerController)) { 
		UE_LOG(LogTemp, Warning, TEXT("Cannot render simulation for player"));
		return;
	}
	APawn* PlayerPawn = PlayerController->GetPawn();
	if (!PlayerPawn)
	{
		UE_LOG(LogTemp, Warning, TEXT("Player pawn is null, cannot render simulation for player"));
		return;
	}
	
	PlayerController->GetCharacter()->GetCharacterMovement()->bIgnoreClientMovementErrorChecksAndCorrection = true;
	PlayerPawn->TeleportTo(Data->SpawnTransform.GetLocation(), Data->SpawnTransform.GetRotation().Rotator());
	PlayerController->GetCharacter()->GetCharacterMovement()->bIgnoreClientMovementErrorChecksAndCorrection = false;
}

void ACaravanInterior::Exit(APlayerController* PlayerController) 
{
	if(!VALIDATE_PARAMS(PlayerController)) { 
		UE_LOG(LogTemp, Warning, TEXT("Cannot stop render simulation for player"));
		return;
	}
	
	APawn* PlayerPawn = PlayerController->GetPawn();
	if (!PlayerPawn)
	{
		UE_LOG(LogTemp, Warning, TEXT("Player pawn is null, cannot render simulation for player"));
		return;
	}
	
	PlayerPawn->SetActorTransform(Data->ExitTransform);
}

bool ACaravanInterior::IsNetRelevantFor(const AActor* RealViewer, const AActor* ViewTarget, const FVector& SrcLocation) const
{
	// Get the player controller for RealViewer
	const APlayerController* RealViewerController = Cast<APlayerController>(RealViewer);
	if (!RealViewerController)
	{
		if (const APawn* RealViewerPawn = Cast<APawn>(RealViewer))
		{
			RealViewerController = RealViewerPawn->GetController<APlayerController>();
		}
	}

	// Get the player controller for ViewTarget
	const APlayerController* ViewTargetController = Cast<APlayerController>(ViewTarget);
	if (!ViewTargetController)
	{
		if (const APawn* ViewTargetPawn = Cast<APawn>(ViewTarget))
		{
			ViewTargetController = ViewTargetPawn->GetController<APlayerController>();
		}
	}

	const UGameInstance* GameInstance = GetWorld()->GetGameInstance();
	if (ULevelsManager* LevelsManager = GameInstance->GetSubsystem<ULevelsManager>())
	{
		return LevelsManager->IsRendering(this, RealViewerController);
	}

	return false;
}
