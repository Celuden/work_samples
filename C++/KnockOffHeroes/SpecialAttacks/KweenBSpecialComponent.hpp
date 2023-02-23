/*
studies: MultiMediaTechnology / FHS
cause: MultiMediaProjekt 2a
team: Alija Suljic, Sabine Steiner, Markus Rinnerthaler
*/

#pragma once
#include "ISpecialAttackComponent.hpp"

namespace mmt_gd
{
class KweenBSpecialComponent final : public ISpecialAttackComponent
{
public:
    KweenBSpecialComponent(GameObject& gameObject, GameObject& beeOne, GameObject& beeTwo, GameObject& beeThree, const int attackValue);

    bool init() override;
    void update(float deltaTime) override;
    void specialAttack() override;
    void stopSpecial() override;

private:
    GameObject& m_beeOne;
    GameObject& m_beeTwo;
    GameObject& m_beeThree;
    float       m_specialTimer;
    const float m_specialTimerLimit;
    bool        m_specialLaunched;
    const int              m_attackValue;
    std::vector<std::string> m_nonBeePlayers;
};
} // namespace mmt_gd