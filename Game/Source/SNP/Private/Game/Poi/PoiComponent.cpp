#include "Game/Poi/PoiComponent.h"

UPoiComponent::UPoiComponent()
{
	// Enable collision
	UPrimitiveComponent::SetCollisionEnabled(ECollisionEnabled::QueryOnly);  // We only need overlap queries, no physics
	UPrimitiveComponent::SetCollisionObjectType(ECollisionChannel::ECC_WorldStatic);  // Or whatever channel you're checking against
    
	// Make sure it responds to queries
	UPrimitiveComponent::SetCollisionResponseToAllChannels(ECR_Ignore);
	UPrimitiveComponent::SetCollisionResponseToChannel(ECC_WorldStatic, ECR_Block);  // Or ECR_Overlap if you prefer
    
	// Make sure it has a shape
	SetGenerateOverlapEvents(true);
    
	// Optional: Make it visible in editor for debugging
	bVisualizeComponent = true;
}