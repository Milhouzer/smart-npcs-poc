#include "Vehicle/SNPVehicle.h"

#include "Components/StaticMeshComponent.h"

ASNPVehicle::ASNPVehicle()
{
	PrimaryActorTick.bCanEverTick = true;

	MovementComponentOverride = CreateDefaultSubobject<USNPVehicleMovementComponent>(TEXT("MovementComponent"));
	MovementComponentOverride->UpdatedComponent = RootComponent;
}

void ASNPVehicle::BeginPlay()
{
	Super::BeginPlay();
}

void ASNPVehicle::Tick(float DeltaTime)
{
	Super::Tick(DeltaTime);
	ApplyMove();
}

UPawnMovementComponent* ASNPVehicle::GetMovementComponent() const
{
	return MovementComponentOverride;
}

void ASNPVehicle::ForwardMovementAction_Implementation(float Value)
{
	AddMovementInput(GetActorForwardVector(), Value);
}

void ASNPVehicle::RightMovementAction_Implementation(float Value)
{
	AddMovementInput(GetActorRightVector(), Value);
}

void ASNPVehicle::CameraUpAction_Implementation(float Value)
{
	AddControllerPitchInput(LookUpDownRate * Value);
}

void ASNPVehicle::CameraRightAction_Implementation(float Value)
{
	AddControllerYawInput(LookLeftRightRate * Value);
}

void ASNPVehicle::Accelerate(float Value)
{
	CurrentSpeed = FMath::Lerp(CurrentSpeed, MaxSpeed, Acceleration);
}

void ASNPVehicle::Turn(float Value)
{
	TurnAmount = FMath::Lerp(TurnAmount, TurningRadius, TurnSpeed);
}

void ASNPVehicle::ApplyMove()
{
	FVector Move = GetActorForwardVector() * CurrentSpeed;
	FRotator Turn = FRotator(TurnAmount, 0, 0);
	AddActorLocalOffset(Move, true);
}


FTransform ASNPVehicle::GetThirdPersonPivotTarget()
{
	return FTransform(GetActorRotation(), GetActorLocation(), FVector::OneVector);
}
