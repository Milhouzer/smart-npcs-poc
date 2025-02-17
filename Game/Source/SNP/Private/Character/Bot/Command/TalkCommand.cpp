#include "Character/Bot/Command/TalkCommand.h"
#include "Character/Bot/Command/Interfaces/CommandRunner.h"
#include "CoreMinimal.h"
#include "Game/GameAPISubsystem.h"

void FTalkCommand::Run(ICommandRunner* Runner)
{
	if (!Runner || !Runner->GetRunner() || !Runner->GetRunner()->HasAuthority())
	{
		UE_LOG(LogTemp, Error, TEXT("Invalid runner state for TalkCommand"));
		return;
	}

	if (UGameInstance* GameInstance = Runner->GetRunner()->GetGameInstance())
	{
		if (UGameAPISubsystem* APISubsystem = GameInstance->GetSubsystem<UGameAPISubsystem>())
		{
			const FTalkRequest* Request = new FTalkRequest(CmdData.Message, CmdData.POIs);
			APISubsystem->SendTalkCommand(*Request, [this](const FTalkResponse& Response, const bool Success)
			{
				if (Success)
				{
					UE_LOG(LogTemp, Display, TEXT("Response to talk command: %s"), *Response.Response);
				}
				else
				{
					UE_LOG(LogTemp, Error, TEXT("Talk command failed: %s"), *Response.Response);
				}
			});
		}
	}
}

void FTalkCommand::InitializeCommand(const FTalkCommandData& Data)
{
	this->CmdData.Name = Data.Name;
	this->CmdData.Message = Data.Message;
	this->CmdData.POIs = Data.POIs;
}

