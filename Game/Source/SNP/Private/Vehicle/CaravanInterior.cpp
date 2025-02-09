#include "Vehicle/CaravanInterior.h"

#include "Game/LevelSimulation/LevelSimulationSubsystem.h"
#include "GameFramework/Character.h"
#include "GameFramework/CharacterMovementComponent.h"
#include "Utils/Utility.h"

ACaravanInterior::ACaravanInterior()
{
	PrimaryActorTick.bCanEverTick = false;
	bReplicates = true;
    
	RootComponent = CreateDefaultSubobject<USceneComponent>(TEXT("SceneComponent"));;
	
	// FloorMeshComponent = CreateDefaultSubobject<UStaticMeshComponent>(TEXT("FloorMeshComponent"));
	// FloorMeshComponent->SetupAttachment(RootComponent);
	// FloorMeshComponent->SetCanEverAffectNavigation(true);
	// FloorMeshComponent->SetVisibility(true, true);
}

void ACaravanInterior::BeginPlay()
{
	Super::BeginPlay();
	// InitializeVisibility();
}


// void ACaravanInterior::InitializeVisibility()
// {
// 	// If we're on a client, wait for OnRep_VisiblePlayers
// 	if (!HasAuthority())
// 	{
// 		FloorMeshComponent->SetVisibility(false, true);
// 		return;
// 	}
//         
// 	// On server, check if we need to update any existing players
// 	if (APlayerController* LocalPlayer = GetWorld()->GetFirstPlayerController())
// 	{
// 		UpdateMeshVisibility(LocalPlayer);
// 	}
// }

// void ACaravanInterior::GetLifetimeReplicatedProps(TArray<FLifetimeProperty>& OutLifetimeProps) const
// {
// 	Super::GetLifetimeReplicatedProps(OutLifetimeProps);
//     
// 	DOREPLIFETIME(ACaravanInterior, VisiblePlayers);
// }

// void ACaravanInterior::SetVisiblePlayer(APlayerController* PlayerController, bool Visible)
// {
//     if (!PlayerController || !HasAuthority())
//     {
//     	UE_LOG(LogTemp, Warning, TEXT("Don't set visibility for player"));
//         return;
//     }
//
//     if (Visible)
//     {
// 		UE_LOG(LogTemp, Log, TEXT("Set visibility for player %p"), PlayerController);
//         VisiblePlayers.AddUnique(PlayerController);
//     }
//     else
//     {
// 		UE_LOG(LogTemp, Log, TEXT("Set invisibility for player %p"), PlayerController);
//         VisiblePlayers.Remove(PlayerController);
//     }
//     
//     // Update visibility immediately on the server
//     UpdateMeshVisibility(PlayerController);
// }
//
// void ACaravanInterior::OnRep_VisiblePlayers()
// {
// 	// Update visibility for the local player on clients
// 	if (APlayerController* LocalPlayer = GetWorld()->GetFirstPlayerController())
// 	{
//     	UE_LOG(LogTemp, Log, TEXT("OnRep_VisiblePlayers"));
// 		UpdateMeshVisibility(LocalPlayer);
// 	}
// }
//
// void ACaravanInterior::UpdateMeshVisibility(APlayerController* PlayerController)
// {
// 	if (!PlayerController || !FloorMeshComponent)
// 	{
// 		UE_LOG(LogTemp, Warning, TEXT("Cannot update mesh visibility for player"));
// 		return;
// 	}
//
// 	const bool bShouldBeVisible = VisiblePlayers.Contains(PlayerController);
// 	UE_LOG(LogTemp, Log, TEXT("Update %p visibility for %p: %hhd"), FloorMeshComponent, PlayerController, bShouldBeVisible);
// 	FloorMeshComponent->SetVisibility(bShouldBeVisible, true);
// }

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
