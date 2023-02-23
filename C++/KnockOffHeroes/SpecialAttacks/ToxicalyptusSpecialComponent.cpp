/*
studies: MultiMediaTechnology / FHS
cause: MultiMediaProjekt 2a
team: Alija Suljic, Sabine Steiner, Markus Rinnerthaler
*/

#include "stdafx.h"

#include "ToxicalyptusSpecialComponent.hpp"

#include "AssetManager.hpp"
#include "HealthComponent.hpp"
#include "RigidBodyComponent.hpp"
#include "SpriteAnimationComponent.hpp"
#include "SpriteRenderComponent.hpp"

mmt_gd::ToxicalyptusSpecialComponent::ToxicalyptusSpecialComponent(
    GameObject&        gameObject,
    GameObject&        poisonHoriPlus,
    GameObject&        poisonVertPlus,
    const sf::Vector2f offset) :
ISpecialAttackComponent(gameObject),
m_poisonHoriPlus(poisonHoriPlus),
m_poisonVertPlus(poisonVertPlus),
m_specialTimer(0.f),
m_specialTimerLimit(2.f),
m_specialLaunched(false)
{
}

bool mmt_gd::ToxicalyptusSpecialComponent::init()
{
    return true;
}

void mmt_gd::ToxicalyptusSpecialComponent::update(float deltaTime)
{
    if (m_specialLaunched)
    {
        m_specialTimer += deltaTime;
        if (m_specialTimer > m_specialTimerLimit)
            stopSpecial();
    }
}

void mmt_gd::ToxicalyptusSpecialComponent::specialAttack()
{
    if (!m_specialLaunched)
    {
        m_specialLaunched = true;

        sf::IntRect textRectHoriPoison = m_poisonHoriPlus.getComponent<SpriteRenderComponent>()->getSprite().getTextureRect();
        sf::IntRect textRectVertPoison = m_poisonVertPlus.getComponent<SpriteRenderComponent>()->getSprite().getTextureRect();
        sf::IntRect textRectPlayer = m_gameObject.getComponent<SpriteRenderComponent>()->getSprite().getTextureRect();

        float heightPosition = m_gameObject.getPosition().y - textRectVertPoison.height / 2 + textRectPlayer.height / 4;
        if (m_gameObject.getComponent<SpriteRenderComponent>()->getSprite().getScale().x < 0)
        {
            sf::Vector2f leftPos = {m_gameObject.getPosition().x - textRectHoriPoison.width / 2 - m_offset.x, heightPosition};
            m_poisonHoriPlus.getComponent<RigidBodyComponent>()->setPosition(leftPos);
            m_poisonVertPlus.getComponent<RigidBodyComponent>()->setPosition(leftPos);
        }
        else
        {
            sf::Vector2f rigthPos = {m_gameObject.getPosition().x - textRectHoriPoison.width / 2 +
                                         textRectPlayer.width +
                                         m_offset.x,
                                     heightPosition};
            m_poisonHoriPlus.getComponent<RigidBodyComponent>()->setPosition(rigthPos);
            m_poisonVertPlus.getComponent<RigidBodyComponent>()->setPosition(rigthPos);
        }
        AssetManager::getInstance().playSound("ToxicalyptusSpecial");

        m_poisonHoriPlus.getComponent<SpriteAnimationComponent>()->resetAnimation();
        m_poisonVertPlus.getComponent<SpriteAnimationComponent>()->resetAnimation();
        m_poisonHoriPlus.setActive(true);
        m_poisonVertPlus.setActive(true);
    }
}

void mmt_gd::ToxicalyptusSpecialComponent::stopSpecial()
{
    m_specialLaunched = false;
    m_specialTimer    = 0;

    m_poisonHoriPlus.setActive(false);
    m_poisonVertPlus.setActive(false);
    m_poisonHoriPlus.getComponent<RigidBodyComponent>()->setPosition(sf::Vector2f(5000, 5000));
    m_poisonVertPlus.getComponent<RigidBodyComponent>()->setPosition(sf::Vector2f(5000, 5000));
}
