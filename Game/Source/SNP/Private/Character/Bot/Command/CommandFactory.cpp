#include "Character/Bot/Command/CommandFactory.h"
#include "Character/Bot/Command/CraftCommand.h"
#include "Character/Bot/Command/TalkCommand.h"
#include "Character/Bot/Command/Interfaces/Command.h"


template <typename CmdType, typename CmdDataType>
TSharedPtr<CmdType> UCommandFactory::CreateCmd(const CmdDataType& CmdData)
{
	static_assert(std::is_base_of_v<ICommand<CmdDataType>, CmdType>,
		"CmdType must derive from ICommand<CmdDataType>");

	TSharedPtr<CmdType> Command = MakeShareable(new CmdType());
	Command->InitializeCommand(CmdData);

	return Command;
}
template TSharedPtr<FCraftCommand> UCommandFactory::CreateCmd<FCraftCommand, FCraftCommandData>(const FCraftCommandData& CmdData);
template TSharedPtr<FTalkCommand> UCommandFactory::CreateCmd<FTalkCommand, FTalkCommandData>(const FTalkCommandData& CmdData);
