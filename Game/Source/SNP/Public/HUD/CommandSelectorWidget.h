#pragma once

#include "CoreMinimal.h"
#include "Blueprint/UserWidget.h"
#include "Character/Bot/Command/Interfaces/CommandRunner.h"
#include "CommandSelectorWidget.generated.h"

/**
 * CommandPayloadInputWidget - A user widget for command payload input with events.
 */
UCLASS(Blueprintable)
class SNP_API UCommandSelectorWidget : public UUserWidget
{
	GENERATED_BODY()
	virtual void NativeConstruct() override;
	
public:	
	// Runner to which the command will be sent
	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	TScriptInterface<ICommandRunner> CommandRunner;
};
