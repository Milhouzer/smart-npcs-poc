#pragma once

#include "CoreMinimal.h"
#include "Interactable.h"
#include "GameFramework/Actor.h"
#include "InteractableComponent.generated.h"

// Delegate declaration for interaction
DECLARE_DYNAMIC_MULTICAST_DELEGATE_OneParam(FOnInteract, AActor*, Interactor);

UCLASS(ClassGroup=(Custom), meta=(BlueprintSpawnableComponent))
class SNP_API UInteractableComponent : public USceneComponent, public IInteractable
{
	GENERATED_BODY()

public:
	UInteractableComponent();

	// Called when interaction occurs
	virtual void Interact(AActor* Interactor) override;

	// Interaction key (e.g., "E" or any input key)
	virtual FText GetInteractionKey() const override;

	// Interaction prompt (e.g., "Press E to Interact")
	virtual FText GetInteractionPrompt() const override;

	// Delegate to bind interaction logic
	UPROPERTY(BlueprintAssignable, Category = "Interaction")
	FOnInteract OnInteract;

protected:
	UPROPERTY(EditDefaultsOnly, Category = "Interaction")
	FText InteractionKey = FText::FromString("E");

	UPROPERTY(EditDefaultsOnly, Category = "Interaction")
	FText InteractionPrompt = FText::FromString("Interact");
};
