#pragma once

#include "CoreMinimal.h"

class ICommandRunner;

/**
 * Generic Command Interface
 */
template <typename TCommandData>
class ICommand
{
public:
	virtual ~ICommand() = default;

	/** Executes the command logic */
	virtual void Run(ICommandRunner* Runner) = 0;

	/** Initialize the command with data */
	virtual void InitializeCommand(const TCommandData& Data) = 0;
};
