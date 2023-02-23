/*
studies: MultiMediaTechnology / FHS
cause: MultiMediaProjekt 2a
team: Alija Suljic, Sabine Steiner, Markus Rinnerthaler
*/

#pragma once
#include "ISpecialAttackComponent.hpp"

namespace mmt_gd
{
class Flop4925SpecialComponent final : public ISpecialAttackComponent
{
public:
    Flop4925SpecialComponent(GameObject& gameObject, GameObject& barrier);

    bool init() override;
    void update(float deltaTime) override;
    void specialAttack() override;
    void stopSpecial() override;

private:
    GameObject& m_barrier;
    float       m_specialTimer;
    const float m_specialTimerLimit;
    bool        m_specialLaunched;
};
} // namespace mmt_gd