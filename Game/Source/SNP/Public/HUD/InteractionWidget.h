#pragma once

#include "CoreMinimal.h"
#include "Blueprint/UserWidget.h"
#include "InteractionWidget.generated.h"

UCLASS()
class SNP_API UInteractionWidget : public UUserWidget
{
	GENERATED_BODY()

public:
	UFUNCTION(BlueprintCallable, Category = "Interaction")
	void SetInteractionKey(const FText& Key);

	UFUNCTION(BlueprintCallable, Category = "Interaction")
	void SetInteractionPrompt(const FText& Prompt);

protected:
	UPROPERTY(meta = (BindWidget))
	class UTextBlock* InteractionKeyText;

	UPROPERTY(meta = (BindWidget))
	class UTextBlock* InteractionPromptText;
};
