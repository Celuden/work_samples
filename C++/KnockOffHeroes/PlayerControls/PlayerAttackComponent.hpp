/*
studies: MultiMediaTechnology / FHS
cause: MultiMediaProjekt 2a
team: Alija Suljic, Sabine Steiner, Markus Rinnerthaler
*/

#pragma once
#include "IComponent.hpp"


namespace mmt_gd
{
class PlayerAttackComponent final : public IComponent
{
public:
    PlayerAttackComponent(GameObject& gameObject, GameObject& attackGO, const int attackValue, const bool inAttackAnimation = false, const float attackLimiter = 0.2f);

    void attack();
    bool init() override;
    void update(float deltaTime) override;

    GameObject& getAttackGO()
    {
        return m_attackGO;
    }

    bool inAttackAnimation()
    {
        return m_inAttackAnimation;
    }

    float m_timeSinceLastAttack{};

private:
    GameObject& m_attackGO;
    const int m_attackValue;
    bool        m_inAttackAnimation;
    const float m_attackLimiter;
};
} // namespace mmt_gd
