/*
studies: MultiMediaTechnology / FHS
cause: MultiMediaProjekt 2a
team: Alija Suljic, Sabine Steiner, Markus Rinnerthaler
*/

#pragma once
#include "ISpecialAttackComponent.hpp"

namespace mmt_gd
{
class ToxicalyptusSpecialComponent final : public ISpecialAttackComponent
{
public:
    ToxicalyptusSpecialComponent(GameObject& gameObject, GameObject& poisonHoriPlus, GameObject& poisonVertPlus, const sf::Vector2f offset);

    bool init() override;
    void update(float deltaTime) override;
    void specialAttack() override;
    void stopSpecial() override;

private:
    GameObject& m_poisonHoriPlus;
    GameObject&        m_poisonVertPlus;
    const sf::Vector2f m_offset;
    float       m_specialTimer;
    const float m_specialTimerLimit;
    bool        m_specialLaunched;
};
} // namespace mmt_gd