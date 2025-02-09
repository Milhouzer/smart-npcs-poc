#include "HUD/InteractionWidget.h"
#include "Components/TextBlock.h"

void UInteractionWidget::SetInteractionKey(const FText& Key)
{
	if (InteractionKeyText)
	{
		InteractionKeyText->SetText(Key);
	}
}

void UInteractionWidget::SetInteractionPrompt(const FText& Prompt)
{
	if (InteractionPromptText)
	{
		InteractionPromptText->SetText(Prompt);
	}
}
