
#pragma once
#include "Bot/SNPBot.h"
#include "Character/ALSCharacter.h"
#include "SNPCharacter.generated.h"


/**
 * Specialized character class, with additional features like held object etc.
 */
UCLASS(Blueprintable, BlueprintType)
class SNP_API ASNPCharacter : public AALSBaseCharacter
{
	GENERATED_BODY()

/************************/
/*			ALS			*/
/************************/
public:
	ASNPCharacter(const FObjectInitializer& ObjectInitializer);

	/** Implemented on BP to update held objects */
	UFUNCTION(BlueprintImplementableEvent, BlueprintCallable, Category = "SNP|HeldObject")
	void UpdateHeldObject();

	UFUNCTION(BlueprintCallable, Category = "SNP|HeldObject")
	void ClearHeldObject();

	UFUNCTION(BlueprintCallable, Category = "SNP|HeldObject")
	void AttachToHand(UStaticMesh* NewStaticMesh, USkeletalMesh* NewSkeletalMesh,
					  class UClass* NewAnimClass, bool bLeftHand, FVector Offset);

	virtual void RagdollStart() override;

	virtual void RagdollEnd() override;

	virtual ECollisionChannel GetThirdPersonTraceParams(FVector& TraceOrigin, float& TraceRadius) override;

	virtual FTransform GetThirdPersonPivotTarget() override;

	virtual FVector GetFirstPersonCameraTarget() override;

protected:
	virtual void Tick(float DeltaTime) override;

	virtual void BeginPlay() override;

	virtual void OnOverlayStateChanged(EALSOverlayState PreviousState) override;

	/** Implement on BP to update animation states of held objects */
	UFUNCTION(BlueprintImplementableEvent, BlueprintCallable, Category = "SNP|HeldObject")
	void UpdateHeldObjectAnimations();

public:
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "SNP|Component")
	TObjectPtr<USceneComponent> HeldObjectRoot = nullptr;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "SNP|Component")
	TObjectPtr<USkeletalMeshComponent> SkeletalMesh = nullptr;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "SNP|Component")
	TObjectPtr<UStaticMeshComponent> StaticMesh = nullptr;

private:
	bool bNeedsColorReset = false;

	
/************************/
/*			SNP			*/
/************************/
public:
	// A TSubclassOf variable to allow BP_Bot selection
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Bot")
	TSubclassOf<ASNPBot> BotBlueprint;
	
	UFUNCTION(BlueprintNativeEvent, BlueprintCallable, Category = "ALS|Input")
	void InteractAction(AActor* Interactable);
	
	// Function to spawn the bot
	UFUNCTION(BlueprintCallable, Category = "Bots")
	void SpawnBot();
protected:
	// Implementation of the interaction logic
	// UFUNCTION(Server, Reliable, WithValidation)
	// void Server_InteractAction(AInteractableActor* Interactable);
	UFUNCTION(Server, Reliable, WithValidation)
	void Server_InteractAction(UInteractableComponent* Interactable);
	
	void ShowPossibleInteraction();
	void ShowInteractionWidget(const UInteractableComponent* Interactable);
	void HideInteractionWidget();
	
	// Reference to the Blueprint subclass of UInteractionWidget
	UPROPERTY(EditDefaultsOnly, BlueprintReadWrite, Category = "Interaction")
	TSubclassOf<class UInteractionWidget> InteractionWidgetClass;

private:
	UPROPERTY(EditDefaultsOnly, Category = "Interaction")
	float InteractionRange = 300.0f;
	
	UPROPERTY()
	TObjectPtr<UInteractableComponent> CurrentInteractable;

	UPROPERTY(Transient)
	class UInteractionWidget* InteractionWidgetInstance;
};