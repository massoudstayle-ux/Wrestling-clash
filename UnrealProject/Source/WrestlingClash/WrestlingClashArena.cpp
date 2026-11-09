#include "WrestlingClashArena.h"
#include "Components/SceneComponent.h"
#include "Components/StaticMeshComponent.h"
#include "Engine/StaticMesh.h"
#include "UObject/ConstructorHelpers.h"

AWrestlingClashArena::AWrestlingClashArena()
{
    PrimaryActorTick.bCanEverTick = false;

    SceneRoot = CreateDefaultSubobject<USceneComponent>(TEXT("SceneRoot"));
    SetRootComponent(SceneRoot);

    static ConstructorHelpers::FObjectFinder<UStaticMesh> CubeAsset(
        TEXT("/Engine/BasicShapes/Cube.Cube"));
    if (CubeAsset.Succeeded())
    {
        CubeMesh = CubeAsset.Object;
    }
}

void AWrestlingClashArena::ClearArena()
{
    for (UStaticMeshComponent* Part : ArenaParts)
    {
        if (Part)
        {
            Part->DestroyComponent();
        }
    }
    ArenaParts.Empty();
}

void AWrestlingClashArena::AddBox(
    const FVector& Location,
    const FVector& Scale,
    const FRotator& Rotation)
{
    if (!CubeMesh)
    {
        return;
    }

    UStaticMeshComponent* Part = NewObject<UStaticMeshComponent>(this);
    if (!Part)
    {
        return;
    }

    Part->SetStaticMesh(CubeMesh);
    Part->SetMobility(EComponentMobility::Movable);
    Part->SetCollisionProfileName(TEXT("BlockAll"));
    Part->SetupAttachment(SceneRoot);
    Part->SetRelativeLocation(Location);
    Part->SetRelativeRotation(Rotation);
    Part->SetRelativeScale3D(Scale);
    Part->RegisterComponent();
    ArenaParts.Add(Part);
}

void AWrestlingClashArena::OnConstruction(const FTransform& Transform)
{
    Super::OnConstruction(Transform);
    ClearArena();

    // Raised square platform and a dark apron.
    AddBox(FVector(0, 0, -25), FVector(6.2f, 6.2f, 0.35f));
    AddBox(FVector(0, 0, -5), FVector(5.7f, 5.7f, 0.08f));

    // Four corner posts.
    const float HalfRing = 270.0f;
    for (int32 XSign : {-1, 1})
    {
        for (int32 YSign : {-1, 1})
        {
            AddBox(FVector(XSign * HalfRing, YSign * HalfRing, 65), FVector(0.18f, 0.18f, 1.3f));
        }
    }

    // Three simple rope rows on each side. These are block-shaped placeholders.
    const float RopeHeights[] = {35.0f, 95.0f, 155.0f};
    for (float Z : RopeHeights)
    {
        AddBox(FVector(0, -HalfRing, Z), FVector(5.4f, 0.045f, 0.045f));
        AddBox(FVector(0, HalfRing, Z), FVector(5.4f, 0.045f, 0.045f));
        AddBox(FVector(-HalfRing, 0, Z), FVector(0.045f, 5.4f, 0.045f));
        AddBox(FVector(HalfRing, 0, Z), FVector(0.045f, 5.4f, 0.045f));
    }
}