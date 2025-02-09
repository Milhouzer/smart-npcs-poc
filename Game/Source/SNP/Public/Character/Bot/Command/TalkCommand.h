#pragma once

#include "Interfaces/BaseCommand.h"
#include "Interfaces/CommandData.h"
#include "Interfaces/CommandRunner.h"

struct FTalkCommandData;
/**
 * FTalkCommand is a specific implementation of TBaseCommand for crafting logic.
 */
class FTalkCommand : public TBaseCommand<FTalkCommandData>
{
public:
	virtual ~FTalkCommand() = default;

	/** Executes the crafting logic for the bot using the given payload. */
	virtual void Run(ICommandRunner* Runner) override;

	/** Sets the data of the command */
	virtual void InitializeCommand(const FTalkCommandData& Data) override;
};
