

#include "Character/SNPCharacter.h"

#include "AI/ALSAIController.h"
#include "Blueprint/UserWidget.h"
#include "Character/SNPPlayerController.h"
#include "Core/InteractableComponent.h"
#include "HUD/InteractionWidget.h"
#include "Utils/Utility.h"

ASNPCharacter::ASNPCharacter(const FObjectInitializer& ObjectInitializer)
	: Super(ObjectInitializer)
{
	HeldObjectRoot = CreateDefaultSubobject<USceneComponent>(TEXT("HeldObjectRoot"));
	HeldObjectRoot->SetupAttachment(GetMesh());

	SkeletalMesh = CreateDefaultSubobject<USkeletalMeshComponent>(TEXT("SkeletalMesh"));
	SkeletalMesh->SetupAttachment(HeldObjectRoot);

	StaticMesh = CreateDefaultSubobject<UStaticMeshComponent>(TEXT("StaticMesh"));
	StaticMesh->SetupAttachment(HeldObjectRoot);

	AIControllerClass = AALSAIController::StaticClass();
}

void ASNPCharacter::ClearHeldObject()
{
	StaticMesh->SetStaticMesh(nullptr);
	SkeletalMesh->SetSkeletalMesh(nullptr);
	SkeletalMesh->SetAnimInstanceClass(nullptr);
}

void ASNPCharacter::AttachToHand(UStaticMesh* NewStaticMesh, USkeletalMesh* NewSkeletalMesh, UClass* NewAnimClass,
                                 bool bLeftHand, FVector Offset)
{
	ClearHeldObject();

	if (IsValid(NewStaticMesh))
	{
		StaticMesh->SetStaticMesh(NewStaticMesh);
	}
	else if (IsValid(NewSkeletalMesh))
	{
		SkeletalMesh->SetSkeletalMesh(NewSkeletalMesh);
		if (IsValid(NewAnimClass))
		{
			SkeletalMesh->SetAnimInstanceClass(NewAnimClass);
		}
	}

	FName AttachBone;
	if (bLeftHand)
	{
		AttachBone = TEXT("VB LHS_ik_hand_gun");
	}
	else
	{
		AttachBone = TEXT("VB RHS_ik_hand_gun");
	}

	HeldObjectRoot->AttachToComponent(GetMesh(),
	                                  FAttachmentTransformRules::SnapToTargetNotIncludingScale, AttachBone);
	HeldObjectRoot->SetRelativeLocation(Offset);
}

void ASNPCharacter::RagdollStart()
{
	ClearHeldObject();
	Super::RagdollStart();
}

void ASNPCharacter::RagdollEnd()
{
	Super::RagdollEnd();
	UpdateHeldObject();
}

ECollisionChannel ASNPCharacter::GetThirdPersonTraceParams(FVector& TraceOrigin, float& TraceRadius)
{
	const FName CameraSocketName = bRightShoulder ? TEXT("TP_CameraTrace_R") : TEXT("TP_CameraTrace_L");
	TraceOrigin = GetMesh()->GetSocketLocation(CameraSocketName);
	TraceRadius = 15.0f;
	return ECC_Camera;
}

FTransform ASNPCharacter::GetThirdPersonPivotTarget()
{
	return FTransform(GetActorRotation(),
	                  (GetMesh()->GetSocketLocation(TEXT("Head")) + GetMesh()->GetSocketLocation(TEXT("root"))) / 2.0f,
	                  FVector::OneVector);
}

FVector ASNPCharacter::GetFirstPersonCameraTarget()
{
	return GetMesh()->GetSocketLocation(TEXT("FP_Camera"));
}

void ASNPCharacter::OnOverlayStateChanged(EALSOverlayState PreviousState)
{
	Super::OnOverlayStateChanged(PreviousState);
	UpdateHeldObject();
}

void ASNPCharacter::InteractAction_Implementation(AActor* Interactable)
{
	
	if (!Interactable)
	{
		UE_LOG(LogTemp, Warning, TEXT("InteractAction called with a null Interactable!"));
		return;
	}

	if (UInteractableComponent* InteractableComponent = Interactable->FindComponentByClass<UInteractableComponent>())
	{
		UE_LOG(LogTemp, Log, TEXT("Interacting with %s"), *Interactable->GetName());
		if (HasAuthority())
		{
			InteractableComponent->Interact(this);
		}
		else
		{
			Server_InteractAction(InteractableComponent);
		}
	}
	else
	{
		UE_LOG(LogTemp, Warning, TEXT("Actor %s does not have a UInteractableComponent!"), *Interactable->GetName());
	}
}

void ASNPCharacter::Server_InteractAction_Implementation(UInteractableComponent* Interactable)
{
	if(!VALIDATE_PARAMS(Interactable)) return;
	
	Interactable->Interact(this);
}

bool ASNPCharacter::Server_InteractAction_Validate(UInteractableComponent* Interactable)
{
	return true;
}

void ASNPCharacter::ShowPossibleInteraction()
{
	const ASNPPlayerController* PC = Cast<ASNPPlayerController>(GetController());
	if (!PC) return;

	if(!PC->ReticleTarget)
	{
		HideInteractionWidget();
		CurrentInteractable = nullptr;
		return;
	}
	
	UInteractableComponent* InteractableComponent = PC->ReticleTarget->FindComponentByClass<UInteractableComponent>();
	if (!InteractableComponent)
	{
		if(!CurrentInteractable) return;
		
		HideInteractionWidget();
		CurrentInteractable = nullptr;
		return;
	}
	
	if (CurrentInteractable != InteractableComponent)
	{
		CurrentInteractable = InteractableComponent;
		ShowInteractionWidget(InteractableComponent);
	}
}

void ASNPCharacter::ShowInteractionWidget(const UInteractableComponent* Interactable)
{
	if(Interactable == nullptr)
	{
		UE_LOG(LogTemp, Warning, TEXT("Interactable is null when setting interaction widget."));

	}
	
	if (!InteractionWidgetInstance)
	{
		this->InteractionWidgetInstance = CreateWidget<UInteractionWidget>(GetWorld(), InteractionWidgetClass);
		this->InteractionWidgetInstance->AddToViewport();
	}

	// UE_LOG(LogTemp, Log, TEXT("Setting interaction widget for Interactable: %s"), *Interactable->GetName());
	this->InteractionWidgetInstance->SetInteractionKey(Interactable->GetInteractionKey());
	this->InteractionWidgetInstance->SetInteractionPrompt(Interactable->GetInteractionPrompt());
}

void ASNPCharacter::HideInteractionWidget()
{
	if (InteractionWidgetInstance)
	{
		InteractionWidgetInstance->RemoveFromParent();
		InteractionWidgetInstance = nullptr;
	}
}

void ASNPCharacter::Tick(float DeltaTime)
{
	Super::Tick(DeltaTime);

	UpdateHeldObjectAnimations();
	ShowPossibleInteraction();
}

void ASNPCharacter::BeginPlay()
{
	Super::BeginPlay();

	UpdateHeldObject();
	if(!HasAuthority()) return;
	SpawnBot();
}

void ASNPCharacter::SpawnBot()
{
	if (BotBlueprint)
	{
		UWorld* World = GetWorld();
		if (World)
		{
			// Spawn parameters
			FActorSpawnParameters SpawnParams;
			SpawnParams.Owner = this;

			// Spawn location and rotation
			FVector SpawnLocation = GetActorLocation() + FVector(100.0f, 0.0f, 0.0f);
			FRotator SpawnRotation = FRotator::ZeroRotator;

			// Spawn the bot
			ASNPBot* Bot = World->SpawnActor<ASNPBot>(BotBlueprint, SpawnLocation, SpawnRotation, SpawnParams);
			UE_LOG(LogTemp, Warning, TEXT("Bot instantiated"));
			Bot->InitializeOwner(this);
		}
	}
}
