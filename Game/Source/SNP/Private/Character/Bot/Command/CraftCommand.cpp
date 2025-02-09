#include "Character/Bot/Command/CraftCommand.h"
#include "Character/Bot/Command/Interfaces/CommandRunner.h"
#include "CoreMinimal.h"

void FCraftCommand::Run(ICommandRunner* Runner)
{
	if(Runner == nullptr)
	{
		UE_LOG(LogTemp, Error, TEXT("Cannot execute %s on null runner"), *CmdData.Name);
		return;
	}
	
	AActor* Actor = Runner->GetRunner();
	if (!(Actor && Actor->HasAuthority()))
	{
		UE_LOG(LogTemp, Error, TEXT("Cannot execute %s on %s"), *CmdData.Name, *Actor->GetName());
		return;
	}

	UE_LOG(LogTemp, Display, TEXT("Execute craft %s command on %s. Quantity payload: %d"), *CmdData.ItemName, *Actor->GetName(), CmdData.Quantity);
	// Send http request using APIClient to python API with pre-made json (!!!! BRAINSTO WITH MB).
	// Pass useful data: current inventory, resources, known poi in order to get the actions that should be made.
	// Pass result to a specific component on the bot that should analyze the response and react in game to it.
}

void FCraftCommand::InitializeCommand(const FCraftCommandData& Data)
{
	this->CmdData.Name = Data.Name;
	this->CmdData.Quantity = Data.Quantity;
	this->CmdData.ItemName = Data.ItemName;
}

