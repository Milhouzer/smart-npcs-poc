#include "Character/SNPPlayerController.h"

#include "EnhancedActionKeyMapping.h"
#include "EnhancedInputComponent.h"
#include "EnhancedInputSubsystems.h"
#include "InputAction.h"
#include "InputMappingContext.h"
#include "GameFramework/Pawn.h"
#include "Character/ALSBaseCharacter.h"
#include "Character/ALSPlayerCameraManager.h"
#include "Character/SNPCharacter.h"
#include "Character/SNPPlayerCameraManager.h"
#include "Components/ALSDebugComponent.h"
#include "Kismet/GameplayStatics.h"

struct FEnhancedActionKeyMapping;

void ASNPPlayerController::BeginPlay()
{
	Super::BeginPlay();
}

void ASNPPlayerController::Tick(float DeltaTime)
{
	Super::Tick(DeltaTime);
	UpdateReticleTarget();
}

void ASNPPlayerController::UpdateReticleTarget()
{
	FHitResult HitResult;
	ACharacter* MyCharacter = UGameplayStatics::GetPlayerCharacter(this, 0);
	if (!MyCharacter) return;
	
	FVector Start = this->PlayerCameraManager->GetCameraLocation();
	FVector Direction = this->PlayerCameraManager->GetCameraRotation().Vector();

	FVector End = Start + ReticleDistance * Direction;

	FCollisionQueryParams Params;
	Params.AddIgnoredActor(MyCharacter);

	bool bHit = GetWorld()->LineTraceSingleByChannel(HitResult, Start, End, ECC_Visibility, Params);

	AActor* NewTarget = bHit ? HitResult.GetActor() : nullptr;

	if (ReticleTarget != NewTarget)
	{
		ReticleTarget = NewTarget;
	}
}

void ASNPPlayerController::OnPossess(APawn* NewPawn)
{
	Super::OnPossess(NewPawn);
	ASNPCharacter* BaseCharacter = Cast<ASNPCharacter>(NewPawn);
	if(BaseCharacter)
	{
		OnPossessCharacter(BaseCharacter);
		return;
	}
	ASNPVehicle* BaseVehicle = Cast<ASNPVehicle>(NewPawn);
	if(BaseVehicle)
	{
		OnPossessVehicle(BaseVehicle);
		return;
	}
}

void ASNPPlayerController::OnPossessCharacter(ASNPCharacter* character)
{
	if (!IsValid(character)) return;

	PossessedCharacter = character;
	PossessedVehicle = nullptr;

	// I have a BP name BP_Bot which inherits from the Character class.
	// I want a parameter that lets me select this BP in the editor and instantiate here
	
	// Necessary for host
	// if (!IsRunningDedicatedServer())
	// {
	// 	// Servers want to setup camera only in listen servers.
	// 	SetupCharacterCamera();
	// }
	//
	// SetupCharacterInputs();
	
	UALSDebugComponent* DebugComp = Cast<UALSDebugComponent>(PossessedCharacter->GetComponentByClass(UALSDebugComponent::StaticClass()));
	if (DebugComp)
	{
		DebugComp->OnPlayerControllerInitialized(this);
	}
}

void ASNPPlayerController::OnPossessVehicle(ASNPVehicle* vehicle)
{
	if(!IsValid(vehicle)) return;

	PossessedVehicle = vehicle;
	PossessedCharacter = nullptr;
}

void ASNPPlayerController::OnRep_Pawn()
{
	Super::OnRep_Pawn();
	PossessedCharacter = Cast<ASNPCharacter>(GetPawn());
	if(PossessedCharacter)
	{
		SetupCharacterCamera();
		SetupCharacterInputs();

		UALSDebugComponent* DebugComp = Cast<UALSDebugComponent>(PossessedCharacter->GetComponentByClass(UALSDebugComponent::StaticClass()));
		if (DebugComp)
		{
			DebugComp->OnPlayerControllerInitialized(this);
		}
		return;
	}

	PossessedVehicle = Cast<ASNPVehicle>(GetPawn());
	if(PossessedVehicle)
	{
		SetupVehicleCamera();
		SetupVehicleInputs();
	}
}

void ASNPPlayerController::SetupInputComponent()
{
	Super::SetupInputComponent();

	UEnhancedInputComponent* EnhancedInputComponent = Cast<UEnhancedInputComponent>(InputComponent);
	if (EnhancedInputComponent)
	{
		EnhancedInputComponent->ClearActionEventBindings();
		EnhancedInputComponent->ClearActionValueBindings();
		EnhancedInputComponent->ClearDebugKeyBindings();

		BindActions(CharacterInputMappingContext);
		BindActions(DebugInputMappingContext);
		
		BindActions(VehicleInputMappingContext);
	}
	else
	{
		UE_LOG(LogTemp, Fatal, TEXT("ALS Community requires Enhanced Input System to be activated in project settings to function properly"));
	}
}

void ASNPPlayerController::BindActions(UInputMappingContext* Context)
{
	if (Context)
	{
		const TArray<FEnhancedActionKeyMapping>& Mappings = Context->GetMappings();
		UEnhancedInputComponent* EnhancedInputComponent = Cast<UEnhancedInputComponent>(InputComponent);
		if (EnhancedInputComponent)
		{
			// There may be more than one keymapping assigned to one action. So, first filter duplicate action entries to prevent multiple delegate bindings
			TSet<const UInputAction*> UniqueActions;
			for (const FEnhancedActionKeyMapping& Keymapping : Mappings)
			{
				UniqueActions.Add(Keymapping.Action);
			}
			for (const UInputAction* UniqueAction : UniqueActions)
			{
				EnhancedInputComponent->BindAction(UniqueAction, ETriggerEvent::Triggered, Cast<UObject>(this), UniqueAction->GetFName());
			}
		}
	}
}

void ASNPPlayerController::SetupCharacterInputs() const
{
	if (PossessedCharacter)
	{
		if (UEnhancedInputLocalPlayerSubsystem* Subsystem = ULocalPlayer::GetSubsystem<UEnhancedInputLocalPlayerSubsystem>(GetLocalPlayer()))
		{
			FModifyContextOptions Options;
			Options.bForceImmediately = 1;
			Subsystem->AddMappingContext(CharacterInputMappingContext, 1, Options);
			UALSDebugComponent* DebugComp = Cast<UALSDebugComponent>(PossessedCharacter->GetComponentByClass(UALSDebugComponent::StaticClass()));
			if (DebugComp)
			{
				// Do only if we have debug component
				Subsystem->AddMappingContext(DebugInputMappingContext, 0, Options);
			}
		}
	}
}

void ASNPPlayerController::SetupCharacterCamera() const
{
	// Call "OnPossess" in Player Camera Manager when possessing a pawn
	AALSPlayerCameraManager* CastedMgr = Cast<AALSPlayerCameraManager>(PlayerCameraManager);
	if (PossessedCharacter && CastedMgr)
	{
		CastedMgr->OnPossess(PossessedCharacter);
	}
}

void ASNPPlayerController::SetupVehicleInputs() const
{
	if (PossessedVehicle)
	{
		if (UEnhancedInputLocalPlayerSubsystem* Subsystem = ULocalPlayer::GetSubsystem<UEnhancedInputLocalPlayerSubsystem>(GetLocalPlayer()))
		{
			FModifyContextOptions Options;
			Options.bForceImmediately = 1;
			Subsystem->RemoveMappingContext(CharacterInputMappingContext, Options);
			// UALSDebugComponent* DebugComp = Cast<UALSDebugComponent>(PossessedCharacter->GetComponentByClass(UALSDebugComponent::StaticClass()));
			// if (DebugComp)
			// {
				// Do only if we have debug component
			Subsystem->RemoveMappingContext(DebugInputMappingContext, Options);
			// }

			Subsystem->AddMappingContext(VehicleInputMappingContext, 1, Options);
		}
	}
}

void ASNPPlayerController::SetupVehicleCamera() const
{
	// Call "OnPossess" in Player Camera Manager when possessing a pawn
	ASNPPlayerCameraManager* CastedMgr = Cast<ASNPPlayerCameraManager>(PlayerCameraManager);
	if (PossessedVehicle && CastedMgr)
	{
		CastedMgr->OnPossess(PossessedVehicle);
	}
}

void ASNPPlayerController::ForwardMovementAction(const FInputActionValue& Value)
{
	if (PossessedCharacter)
	{
		PossessedCharacter->ForwardMovementAction(Value.GetMagnitude());
	}

	if(PossessedVehicle)
	{
		PossessedVehicle->ForwardMovementAction(Value.GetMagnitude());
	}
}

void ASNPPlayerController::RightMovementAction(const FInputActionValue& Value)
{
	if (PossessedCharacter)
	{
		PossessedCharacter->RightMovementAction(Value.GetMagnitude());
	}
	
	if(PossessedVehicle)
	{
		PossessedVehicle->RightMovementAction(Value.GetMagnitude());
	}
}

void ASNPPlayerController::CameraUpAction(const FInputActionValue& Value)
{
	if (PossessedCharacter)
	{
		PossessedCharacter->CameraUpAction(Value.GetMagnitude());
	}
	
	if(PossessedVehicle)
	{
		PossessedVehicle->CameraUpAction(Value.GetMagnitude());
	}
}

void ASNPPlayerController::CameraRightAction(const FInputActionValue& Value)
{
	if (PossessedCharacter)
	{
		PossessedCharacter->CameraRightAction(Value.GetMagnitude());
	}

	if(PossessedVehicle)
	{
		PossessedVehicle->CameraRightAction(Value.GetMagnitude());
	}
}

void ASNPPlayerController::JumpAction(const FInputActionValue& Value)
{
	if (PossessedCharacter)
	{
		PossessedCharacter->JumpAction(Value.Get<bool>());
	}
}

void ASNPPlayerController::SprintAction(const FInputActionValue& Value)
{
	if (PossessedCharacter)
	{
		PossessedCharacter->SprintAction(Value.Get<bool>());
	}
}

void ASNPPlayerController::AimAction(const FInputActionValue& Value)
{
	if (PossessedCharacter)
	{
		PossessedCharacter->AimAction(Value.Get<bool>());
	}
}

void ASNPPlayerController::InteractAction(const FInputActionValue& Value)
{
	if(ReticleTarget == nullptr) return;
	PossessedCharacter->InteractAction(ReticleTarget);
	
	// Delegate logic to BP
	// APawn* PawnTarget = Cast<APawn>(ReticleTarget);
	// if(PawnTarget)
	// {
	// 	Server_PossessTarget(PawnTarget);	
	// }
}

void ASNPPlayerController::Server_PossessTarget_Implementation(APawn* Target)
{
	if (Target && Target != GetPawn())
	{
		UE_LOG(LogTemp, Log, TEXT("Possess new pawn: Current Pawn = %s, Target Pawn = %s"), 
			*GetPawn()->GetName(), *Target->GetName());
		UnPossess();
		Possess(Target);
	}
}

bool ASNPPlayerController::Server_PossessTarget_Validate(APawn* Target)
{
	// Add validation if necessary, like checking if the target is a valid pawn
	return true;
}

void ASNPPlayerController::CameraTapAction(const FInputActionValue& Value)
{
	if (PossessedCharacter)
	{
		PossessedCharacter->CameraTapAction();
	}
}

void ASNPPlayerController::CameraHeldAction(const FInputActionValue& Value)
{
	if (PossessedCharacter)
	{
		PossessedCharacter->CameraHeldAction();
	}
}

void ASNPPlayerController::StanceAction(const FInputActionValue& Value)
{
	if (PossessedCharacter && Value.Get<bool>())
	{
		PossessedCharacter->StanceAction();
	}
}

void ASNPPlayerController::WalkAction(const FInputActionValue& Value)
{
	if (PossessedCharacter && Value.Get<bool>())
	{
		PossessedCharacter->WalkAction();
	}
}

void ASNPPlayerController::RagdollAction(const FInputActionValue& Value)
{
	if (PossessedCharacter && Value.Get<bool>())
	{
		PossessedCharacter->RagdollAction();
	}
}

void ASNPPlayerController::VelocityDirectionAction(const FInputActionValue& Value)
{
	if (PossessedCharacter && Value.Get<bool>())
	{
		PossessedCharacter->VelocityDirectionAction();
	}
}

void ASNPPlayerController::LookingDirectionAction(const FInputActionValue& Value)
{
	if (PossessedCharacter && Value.Get<bool>())
	{
		PossessedCharacter->LookingDirectionAction();
	}
}

void ASNPPlayerController::DebugToggleHudAction(const FInputActionValue& Value)
{
	if (PossessedCharacter && Value.Get<bool>())
	{
		UALSDebugComponent* DebugComp = Cast<UALSDebugComponent>(PossessedCharacter->GetComponentByClass(UALSDebugComponent::StaticClass()));
		if (DebugComp)
		{
			DebugComp->ToggleHud();
		}
	}
}

void ASNPPlayerController::DebugToggleDebugViewAction(const FInputActionValue& Value)
{
	if (PossessedCharacter && Value.Get<bool>())
	{
		UALSDebugComponent* DebugComp = Cast<UALSDebugComponent>(PossessedCharacter->GetComponentByClass(UALSDebugComponent::StaticClass()));
		if (DebugComp)
		{
			DebugComp->ToggleDebugView();
		}
	}
}

void ASNPPlayerController::DebugToggleTracesAction(const FInputActionValue& Value)
{
	if (PossessedCharacter && Value.Get<bool>())
	{
		UALSDebugComponent* DebugComp = Cast<UALSDebugComponent>(PossessedCharacter->GetComponentByClass(UALSDebugComponent::StaticClass()));
		if (DebugComp)
		{
			DebugComp->ToggleTraces();
		}
	}
}

void ASNPPlayerController::DebugToggleShapesAction(const FInputActionValue& Value)
{
	if (PossessedCharacter && Value.Get<bool>())
	{
		UALSDebugComponent* DebugComp = Cast<UALSDebugComponent>(PossessedCharacter->GetComponentByClass(UALSDebugComponent::StaticClass()));
		if (DebugComp)
		{
			DebugComp->ToggleDebugShapes();
		}
	}
}

void ASNPPlayerController::DebugToggleLayerColorsAction(const FInputActionValue& Value)
{
	if (PossessedCharacter && Value.Get<bool>())
	{
		UALSDebugComponent* DebugComp = Cast<UALSDebugComponent>(PossessedCharacter->GetComponentByClass(UALSDebugComponent::StaticClass()));
		if (DebugComp)
		{
			DebugComp->ToggleLayerColors();
		}
	}
}

void ASNPPlayerController::DebugToggleCharacterInfoAction(const FInputActionValue& Value)
{
	if (PossessedCharacter && Value.Get<bool>())
	{
		UALSDebugComponent* DebugComp = Cast<UALSDebugComponent>(PossessedCharacter->GetComponentByClass(UALSDebugComponent::StaticClass()));
		if (DebugComp)
		{
			DebugComp->ToggleCharacterInfo();
		}
	}
}

void ASNPPlayerController::DebugToggleSlomoAction(const FInputActionValue& Value)
{
	if (PossessedCharacter && Value.Get<bool>())
	{
		UALSDebugComponent* DebugComp = Cast<UALSDebugComponent>(PossessedCharacter->GetComponentByClass(UALSDebugComponent::StaticClass()));
		if (DebugComp)
		{
			DebugComp->ToggleSlomo();
		}
	}
}

void ASNPPlayerController::DebugFocusedCharacterCycleAction(const FInputActionValue& Value)
{
	if (PossessedCharacter)
	{
		UALSDebugComponent* DebugComp = Cast<UALSDebugComponent>(PossessedCharacter->GetComponentByClass(UALSDebugComponent::StaticClass()));
		if (DebugComp)
		{
			DebugComp->FocusedDebugCharacterCycle(Value.GetMagnitude() > 0);
		}
	}
}

void ASNPPlayerController::DebugToggleMeshAction(const FInputActionValue& Value)
{
	if (PossessedCharacter && Value.Get<bool>())
	{
		UALSDebugComponent* DebugComp = Cast<UALSDebugComponent>(PossessedCharacter->GetComponentByClass(UALSDebugComponent::StaticClass()));
		if (DebugComp)
		{
			DebugComp->ToggleDebugMesh();
		}
	}
}

void ASNPPlayerController::DebugOpenOverlayMenuAction(const FInputActionValue& Value)
{
	if (PossessedCharacter)
	{
		UALSDebugComponent* DebugComp = Cast<UALSDebugComponent>(PossessedCharacter->GetComponentByClass(UALSDebugComponent::StaticClass()));
		if (DebugComp)
		{
			DebugComp->OpenOverlayMenu(Value.Get<bool>());
		}
	}
}

void ASNPPlayerController::DebugOverlayMenuCycleAction(const FInputActionValue& Value)
{
	if (PossessedCharacter)
	{
		UALSDebugComponent* DebugComp = Cast<UALSDebugComponent>(PossessedCharacter->GetComponentByClass(UALSDebugComponent::StaticClass()));
		if (DebugComp)
		{
			DebugComp->OverlayMenuCycle(Value.GetMagnitude() > 0);
		}
	}
}
