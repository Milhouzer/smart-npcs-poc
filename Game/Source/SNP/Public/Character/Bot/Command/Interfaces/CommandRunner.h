#pragma once

#include "Command.h"
#include "CommandData.h"
#include "CommandRunner.generated.h"

UINTERFACE(Blueprintable)
class SNP_API UCommandRunner : public UInterface
{
	GENERATED_BODY()
};

class SNP_API ICommandRunner
{
	GENERATED_BODY()

public:
	/** Get the actor running the command **/
	virtual AActor* GetRunner()
	{
		return nullptr;
	}
	
	/** Run the specific command.
	 *
	 * Remarks:
	 * This is business logic and has nothing to do here. Delegate logic to current implementation of ICommandRunner
	 */
	virtual void Execute(const FString& CmdName, const FCommandData& CmdData) = 0;
};