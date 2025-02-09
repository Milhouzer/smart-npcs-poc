#pragma once

#include "CoreMinimal.h"
#include "UObject/Interface.h"
#include "Interactable.generated.h"

UINTERFACE(MinimalAPI)
class UInteractable : public UInterface
{
	GENERATED_BODY()
};

class SNP_API IInteractable
{
	GENERATED_BODY()

public:
	// Function to trigger interaction
	virtual void Interact(AActor* Interactor) = 0;

	// Function to get interaction key for the widget
	virtual FText GetInteractionKey() const = 0;

	// Function to get interaction prompt text
	virtual FText GetInteractionPrompt() const = 0;
};
