#pragma once

#include "Interfaces/BaseCommand.h"
#include "Interfaces/CommandData.h"
#include "Interfaces/CommandRunner.h"

struct FCraftCommandData;
/**
 * FCraftCommand is a specific implementation of TBaseCommand for crafting logic.
 */
class FCraftCommand : public TBaseCommand<FCraftCommandData>
{
public:
	virtual ~FCraftCommand() = default;

	/** Executes the crafting logic for the bot using the given payload. */
	virtual void Run(ICommandRunner* Runner) override;

	/** Sets the data of the command */
	virtual void InitializeCommand(const FCraftCommandData& Data) override;
};
