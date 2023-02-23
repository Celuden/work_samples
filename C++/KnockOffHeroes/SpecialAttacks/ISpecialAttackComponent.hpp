/*
studies: MultiMediaTechnology / FHS
cause: MultiMediaProjekt 2a
team: Alija Suljic, Sabine Steiner, Markus Rinnerthaler
*/

#pragma once
#include "GameObject.hpp"

namespace mmt_gd
{
class ISpecialAttackComponent : public IComponent
{
public:
    explicit ISpecialAttackComponent(GameObject& gameObject) : IComponent(gameObject)
    {
    }

    virtual ~ISpecialAttackComponent()                                  = default;
    ISpecialAttackComponent(ISpecialAttackComponent& other)             = default;
    ISpecialAttackComponent(ISpecialAttackComponent&& other)            = default;
    ISpecialAttackComponent& operator=(ISpecialAttackComponent& other)  = delete;
    ISpecialAttackComponent& operator=(ISpecialAttackComponent&& other) = delete;

    virtual bool init()                  = 0;
    virtual void update(float deltaTime) = 0;
    virtual void specialAttack()         = 0;
    virtual void stopSpecial()           = 0;
};
} // namespace mmt_gd
