#pragma once

#include "Poi.generated.h"

UINTERFACE(Blueprintable)
class SNP_API UPoi : public UInterface
{
	GENERATED_BODY()
};

class SNP_API IPoi
{
	GENERATED_BODY()
	
public:
	/** Get the actor running the command **/
	virtual float GetRadius()
	{
		return 0;
	}
};
