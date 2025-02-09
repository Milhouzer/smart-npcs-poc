#include "Character/Bot/SNPBot.h"
#include "Character/Bot/Command/CommandFactory.h"
#include "Character/Bot/Command/CraftCommand.h"
#include "Character/Bot/Command/TalkCommand.h"
#include "Core/InteractableComponent.h"

ASNPBot::ASNPBot()
{
	bReplicates = true;
	PrimaryActorTick.bCanEverTick = false;
	InteractableCmp = CreateDefaultSubobject<UInteractableComponent>("Interactable");
}

void ASNPBot::InitializeOwner(AActor* BotOwner)
{
	UE_LOG(LogTemp, Warning, TEXT("Bot initialized"));
	OnInitialized.Broadcast(BotOwner);
}

AActor* ASNPBot::GetRunner()
{
	return this;
}

void ASNPBot::Execute(const FString& CmdName, const FCommandData& CmdData)
{
	if (CmdName == CRAFT_COMMAND_NAME)
	{
		const FCraftCommandData& CraftCommandData = static_cast<const FCraftCommandData&>(CmdData);
		ServerExecuteCraftCommand(CmdName, CraftCommandData);
	}
	if (CmdName == TALK_COMMAND_NAME)
	{
		const FTalkCommandData& TalkCommandData = static_cast<const FTalkCommandData&>(CmdData);
		ServerExecuteTalkCommand(CmdName, "TalkCommandData");
	}
}

void ASNPBot::ServerExecuteCraftCommand_Implementation(const FString& CmdName, const FCraftCommandData& CommandData)
{
	UE_LOG(LogTemp, Log, TEXT("Execute %s on %s"), *this->GetName(), *CommandData.Name);
	auto Command = UCommandFactory::CreateCmd<FCraftCommand>(CommandData);
	Command->Run(this);
}

bool ASNPBot::ServerExecuteCraftCommand_Validate(const FString& CmdName, const FCraftCommandData& CommandData)
{
	return true;
}


void ASNPBot::ServerExecuteTalkCommand_Implementation(const FString& CmdName, const FString& Message)
{
	TArray<UPoiComponent*> AllPOIs = CheckForNearbyPOIs();
	TArray<FString> NearbyPOIs;
	Algo::Transform(
		AllPOIs,
		NearbyPOIs,
		[&](UPoiComponent* POI) { return POI->Name; }
	);

	FTalkCommandData* TalkCommandData = new FTalkCommandData(Message, NearbyPOIs);
	UE_LOG(LogTemp, Log, TEXT("Execute %s on %s"), *this->GetName(), *TalkCommandData->Name);
	const auto Command = UCommandFactory::CreateCmd<FTalkCommand>(*TalkCommandData);
	Command->Run(this);
}

bool ASNPBot::ServerExecuteTalkCommand_Validate(const FString& CmdName, const FString& Message)
{
	return true;
}
