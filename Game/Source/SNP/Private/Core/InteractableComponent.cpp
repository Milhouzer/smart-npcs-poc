#include "Core/InteractableComponent.h"
#include "GameFramework/PlayerController.h"

UInteractableComponent::UInteractableComponent()
{
	PrimaryComponentTick.bCanEverTick = false;
}

void UInteractableComponent::Interact(AActor* Interactor)
{
	if (Interactor)
	{
		UE_LOG(LogTemp, Log, TEXT("%s interacted with %s"), *GetNameSafe(Interactor), *GetNameSafe(this));

		// Trigger the delegate
		OnInteract.Broadcast(Interactor);
	}
}

FText UInteractableComponent::GetInteractionKey() const
{
	return InteractionKey;
}

FText UInteractableComponent::GetInteractionPrompt() const
{
	return InteractionPrompt;
}
