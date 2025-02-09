#pragma once

#include "CoreMinimal.h"
#include "Poi.h"
#include "PoiComponent.generated.h"

UCLASS(ClassGroup=(Custom), meta=(BlueprintSpawnableComponent))
class SNP_API UPoiComponent : public UPrimitiveComponent, public IPoi  // Changed from USceneComponent
{
	GENERATED_BODY()

public:
	UPoiComponent();

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "POI")
	FString Name = "DefaultPOI";
	
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "POI")
	float Radius = 1000.0f;

protected:
	// Override these to set up collision
	virtual FPrimitiveSceneProxy* CreateSceneProxy() override { return nullptr; }
	virtual FBoxSphereBounds CalcBounds(const FTransform& LocalToWorld) const override
	{
		return FBoxSphereBounds(LocalToWorld.GetLocation(), FVector(Radius), Radius);
	}
};