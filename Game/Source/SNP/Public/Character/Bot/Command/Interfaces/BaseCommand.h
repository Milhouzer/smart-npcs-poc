#pragma once

#include <memory>
#include "Command.h"

/**
 * Base class for commands, providing common functionality
 */
template <typename CmdDataType>
class TBaseCommand : public ICommand<CmdDataType>
{
public:
	virtual ~TBaseCommand() = default;

	/** Executes the command logic */
	virtual void Run(ICommandRunner* Runner) override
	{
		// Default implementation can be empty or provide base functionality
	}

	/** Sets the data for the command */
	virtual void InitializeCommand(const CmdDataType& Data) override
	{
		CmdData = Data;
	}

protected:
	/** Command data payload */
	CmdDataType CmdData;
};
