#pragma once

#include "CoreMinimal.h"
#include "GameFramework/Actor.h"
#include "WrestlingClashArena.generated.h"

class UStaticMesh;
class UStaticMeshComponent;

UCLASS()
class WRESTLINGCLASH_API AWrestlingClashArena : public AActor
{
    GENERATED_BODY()

public:
    AWrestlingClashArena();

protected:
    virtual void OnConstruction(const FTransform& Transform) override;

private:
    UPROPERTY()
    TObjectPtr<USceneComponent> SceneRoot;

    UPROPERTY()
    TObjectPtr<UStaticMesh> CubeMesh;

    UPROPERTY()
    TArray<TObjectPtr<UStaticMeshComponent>> ArenaParts;

    void ClearArena();
    void AddBox(const FVector& Location, const FVector& Scale, const FRotator& Rotation = FRotator::ZeroRotator);
};