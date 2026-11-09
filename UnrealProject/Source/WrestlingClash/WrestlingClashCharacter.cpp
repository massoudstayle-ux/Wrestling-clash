#include "WrestlingClashCharacter.h"
#include "Camera/CameraComponent.h"
#include "Components/CapsuleComponent.h"
#include "Components/StaticMeshComponent.h"
#include "Engine/StaticMesh.h"
#include "Engine/World.h"
#include "Engine/Engine.h"
#include "EngineUtils.h"
#include "GameFramework/CharacterMovementComponent.h"
#include "GameFramework/SpringArmComponent.h"
#include "GameFramework/Controller.h"
#include "Components/InputComponent.h"
#include "UObject/ConstructorHelpers.h"

AWrestlingClashCharacter::AWrestlingClashCharacter()
{
    PrimaryActorTick.bCanEverTick = true;

    GetCapsuleComponent()->InitCapsuleSize(42.0f, 96.0f);
    bUseControllerRotationPitch = false;
    bUseControllerRotationRoll = false;
    bUseControllerRotationYaw = false;

    UCharacterMovementComponent* Movement = GetCharacterMovement();
    Movement->bOrientRotationToMovement = true;
    Movement->RotationRate = FRotator(0.0f, 540.0f, 0.0f);
    Movement->MaxWalkSpeed = 420.0f;
    Movement->BrakingDecelerationWalking = 1800.0f;
    Movement->AirControl = 0.25f;

    static ConstructorHelpers::FObjectFinder<UStaticMesh> CubeAsset(
        TEXT("/Engine/BasicShapes/Cube.Cube"));
    static ConstructorHelpers::FObjectFinder<UStaticMesh> SphereAsset(
        TEXT("/Engine/BasicShapes/Sphere.Sphere"));

    auto SetupPart = [this](UStaticMeshComponent* Part, UStaticMesh* Mesh,
                            const FVector& Position, const FVector& Scale)
    {
        if (Part && Mesh)
        {
            Part->SetStaticMesh(Mesh);
            Part->SetupAttachment(RootComponent);
            Part->SetRelativeLocation(Position);
            Part->SetRelativeScale3D(Scale);
            Part->SetCollisionEnabled(ECollisionEnabled::NoCollision);
        }
    };

    TorsoMesh = CreateDefaultSubobject<UStaticMeshComponent>(TEXT("PlaceholderTorso"));
    SetupPart(TorsoMesh, CubeAsset.Succeeded() ? CubeAsset.Object : nullptr,
              FVector(0, 0, 0), FVector(0.38f, 0.28f, 0.55f));

    HeadMesh = CreateDefaultSubobject<UStaticMeshComponent>(TEXT("PlaceholderHead"));
    SetupPart(HeadMesh, SphereAsset.Succeeded() ? SphereAsset.Object : nullptr,
              FVector(0, 0, 78), FVector(0.23f, 0.23f, 0.25f));

    LeftArmMesh = CreateDefaultSubobject<UStaticMeshComponent>(TEXT("PlaceholderLeftArm"));
    SetupPart(LeftArmMesh, CubeAsset.Succeeded() ? CubeAsset.Object : nullptr,
              FVector(0, -53, 18), FVector(0.15f, 0.15f, 0.38f));

    RightArmMesh = CreateDefaultSubobject<UStaticMeshComponent>(TEXT("PlaceholderRightArm"));
    SetupPart(RightArmMesh, CubeAsset.Succeeded() ? CubeAsset.Object : nullptr,
              FVector(0, 53, 18), FVector(0.15f, 0.15f, 0.38f));

    LeftLegMesh = CreateDefaultSubobject<UStaticMeshComponent>(TEXT("PlaceholderLeftLeg"));
    SetupPart(LeftLegMesh, CubeAsset.Succeeded() ? CubeAsset.Object : nullptr,
              FVector(0, -18, -69), FVector(0.16f, 0.16f, 0.28f));

    RightLegMesh = CreateDefaultSubobject<UStaticMeshComponent>(TEXT("PlaceholderRightLeg"));
    SetupPart(RightLegMesh, CubeAsset.Succeeded() ? CubeAsset.Object : nullptr,
              FVector(0, 18, -69), FVector(0.16f, 0.16f, 0.28f));

    CameraBoom = CreateDefaultSubobject<USpringArmComponent>(TEXT("CameraBoom"));
    CameraBoom->SetupAttachment(RootComponent);
    CameraBoom->TargetArmLength = 650.0f;
    CameraBoom->SetRelativeLocation(FVector(0.0f, 0.0f, 110.0f));
    CameraBoom->bUsePawnControlRotation = true;
    CameraBoom->bEnableCameraLag = true;
    CameraBoom->CameraLagSpeed = 8.0f;

    FollowCamera = CreateDefaultSubobject<UCameraComponent>(TEXT("FollowCamera"));
    FollowCamera->SetupAttachment(CameraBoom, USpringArmComponent::SocketName);
    FollowCamera->bUsePawnControlRotation = false;
}

void AWrestlingClashCharacter::Tick(float DeltaSeconds)
{
    Super::Tick(DeltaSeconds);

    if (!bIsTrainingOpponent || !GetWorld() || Health <= 0.0f)
    {
        return;
    }

    AWrestlingClashCharacter* Target = nullptr;
    float BestDistance = TNumericLimits<float>::Max();

    for (TActorIterator<AWrestlingClashCharacter> It(GetWorld()); It; ++It)
    {
        AWrestlingClashCharacter* Candidate = *It;
        if (!Candidate || Candidate == this || Candidate->GetHealth() <= 0.0f)
        {
            continue;
        }

        const float CandidateDistance = FVector::Dist(
            GetActorLocation(), Candidate->GetActorLocation());
        if (CandidateDistance < BestDistance)
        {
            BestDistance = CandidateDistance;
            Target = Candidate;
        }
    }

    if (!Target)
    {
        return;
    }

    FVector ToTarget = Target->GetActorLocation() - GetActorLocation();
    ToTarget.Z = 0.0f;
    const FVector Direction = ToTarget.GetSafeNormal();
    if (Direction.IsNearlyZero())
    {
        return;
    }

    const FRotator DesiredRotation = Direction.Rotation();
    SetActorRotation(FMath::RInterpTo(
        GetActorRotation(), DesiredRotation, DeltaSeconds, 5.0f));

    if (BestDistance > 155.0f)
    {
        AddMovementInput(Direction, 0.65f);
    }
    else if (GetWorld()->GetTimeSeconds() - LastAttackTime >= 0.9f)
    {
        Punch();
    }
}

void AWrestlingClashCharacter::SetupPlayerInputComponent(UInputComponent* PlayerInputComponent)
{
    Super::SetupPlayerInputComponent(PlayerInputComponent);

    PlayerInputComponent->BindAxis(TEXT("MoveForward"), this, &AWrestlingClashCharacter::MoveForward);
    PlayerInputComponent->BindAxis(TEXT("MoveRight"), this, &AWrestlingClashCharacter::MoveRight);
    PlayerInputComponent->BindAxis(TEXT("Turn"), this, &APawn::AddControllerYawInput);
    PlayerInputComponent->BindAxis(TEXT("LookUp"), this, &APawn::AddControllerPitchInput);
    PlayerInputComponent->BindAction(TEXT("Jump"), IE_Pressed, this, &ACharacter::Jump);
    PlayerInputComponent->BindAction(TEXT("Jump"), IE_Released, this, &ACharacter::StopJumping);
    PlayerInputComponent->BindAction(TEXT("Punch"), IE_Pressed, this, &AWrestlingClashCharacter::Punch);
    PlayerInputComponent->BindAction(TEXT("Kick"), IE_Pressed, this, &AWrestlingClashCharacter::Kick);
}

void AWrestlingClashCharacter::SetTouchMoveInput(float Forward, float Right)
{
    // UMG joystick widgets can call this while the stick is held.
    MoveForward(FMath::Clamp(Forward, -1.0f, 1.0f));
    MoveRight(FMath::Clamp(Right, -1.0f, 1.0f));
}

void AWrestlingClashCharacter::TouchPunch()
{
    Punch();
}

void AWrestlingClashCharacter::TouchKick()
{
    Kick();
}

void AWrestlingClashCharacter::TouchLook(float YawDelta, float PitchDelta)
{
    AddControllerYawInput(YawDelta);
    AddControllerPitchInput(PitchDelta);
}

void AWrestlingClashCharacter::MoveForward(float Value)
{
    if (Controller && FMath::Abs(Value) > KINDA_SMALL_NUMBER)
    {
        const FRotator YawRotation(0.0f, Controller->GetControlRotation().Yaw, 0.0f);
        AddMovementInput(FRotationMatrix(YawRotation).GetUnitAxis(EAxis::X), Value);
    }
}

void AWrestlingClashCharacter::MoveRight(float Value)
{
    if (Controller && FMath::Abs(Value) > KINDA_SMALL_NUMBER)
    {
        const FRotator YawRotation(0.0f, Controller->GetControlRotation().Yaw, 0.0f);
        AddMovementInput(FRotationMatrix(YawRotation).GetUnitAxis(EAxis::Y), Value);
    }
}

void AWrestlingClashCharacter::Punch()
{
    PerformAttack(12.0f, 185.0f);
}

void AWrestlingClashCharacter::Kick()
{
    PerformAttack(18.0f, 210.0f);
}

void AWrestlingClashCharacter::PerformAttack(float Damage, float Range)
{
    if (!GetWorld() || GetWorld()->GetTimeSeconds() - LastAttackTime < 0.45f)
    {
        return;
    }

    LastAttackTime = GetWorld()->GetTimeSeconds();

    for (TActorIterator<AWrestlingClashCharacter> It(GetWorld()); It; ++It)
    {
        AWrestlingClashCharacter* Other = *It;
        if (!Other || Other == this || Other->Health <= 0.0f)
        {
            continue;
        }

        const FVector ToTarget = Other->GetActorLocation() - GetActorLocation();
        const float Distance = ToTarget.Size();
        const float Facing = FVector::DotProduct(GetActorForwardVector(), ToTarget.GetSafeNormal());

        if (Distance <= Range && Facing >= 0.25f)
        {
            Other->ReceiveHit(Damage);
            if (GEngine)
            {
                GEngine->AddOnScreenDebugMessage(
                    -1, 1.5f, FColor::Yellow,
                    FString::Printf(TEXT("Hit! Opponent health: %.0f"), Other->Health));
            }
            return;
        }
    }
}

void AWrestlingClashCharacter::ReceiveHit(float Damage)
{
    Health = FMath::Max(0.0f, Health - Damage);
    if (Health <= 0.0f)
    {
        GetCharacterMovement()->DisableMovement();
        UE_LOG(LogTemp, Display, TEXT("%s has been defeated."), *GetName());
    }
}