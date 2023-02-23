/*
studies: MultiMediaTechnology / FHS
cause: MultiMediaProjekt 2a
team: Alija Suljic, Sabine Steiner, Markus Rinnerthaler
*/

#include "stdafx.h"

#include "RigidBodyComponent.hpp"

#include "BoxCollisionComponent.hpp"
#include "PhysicsManager.hpp"
#include "PlayerMoveComponent.hpp"
#include "SpriteAnimationComponent.hpp"

mmt_gd::RigidBodyComponent::RigidBodyComponent(GameObject& gameObject, float mass) :
IComponent(gameObject),
m_position(gameObject.getPosition()),
m_mass(std::move(mass)),
m_inverseMass(m_mass == 0.f ? 0.f : 1.f / m_mass),
m_colliders()
{
    PhysicsManager::getInstance().addRigidBodyComponent(this);
}

bool mmt_gd::RigidBodyComponent::init()
{
    // Save all colliders of rigid body with their m_isTrigger bools
    for (auto& colliderComp : m_gameObject.getComponents<BoxCollisionComponent>())
    {
        m_colliders.insert(
            std::pair<bool, std::shared_ptr<BoxCollisionComponent>>(colliderComp->m_isTrigger, colliderComp));
    }

    // If mass is insanely high, set inverseMass to "infinite"
    if (m_mass > 300)
    {
        m_inverseMass = 0;
    }

    return true;
}

void mmt_gd::RigidBodyComponent::update(float deltaTime)
{
}

void mmt_gd::RigidBodyComponent::onCollision(Manifold& man)
{
    // Calculate relative velocity
    sf::Vector2f rv = man.m_body2->m_velocity - man.m_body1->m_velocity;
    // Calculate relative velocity in terms of the normal direction
    float velAlongNormal = rv.x * man.m_normal.x + rv.y * man.m_normal.y;
    // Do not resolve if velocities are separating
    if (velAlongNormal < 0)
        return;
    // Calculate impulse scalar
    float j = velAlongNormal;
    j /= man.m_body1->m_inverseMass + man.m_body2->m_inverseMass;
    // Apply impulse
    sf::Vector2f impulse    = j * man.m_normal;
    man.m_body1->m_velocity = man.m_body1->m_velocity + man.m_body1->m_inverseMass * impulse;
    man.m_body2->m_velocity = man.m_body2->m_velocity - man.m_body2->m_inverseMass * impulse;

    PhysicsManager::getInstance().positionalCorrection(man.m_body1, man.m_body2, man.m_penetration, man.m_normal);

    if ((man.m_body1->getGameObject().getTag() == GameObjectTag::Player) &&
        (man.m_body2->getGameObject().getTag() == GameObjectTag::Platform) &&
        !man.m_body1->getGameObject().getComponent<PlayerMoveComponent>()->getJumpTimerActive())
    {
        man.m_body1->getGameObject().getComponent<PlayerMoveComponent>()->setGrounded(true);
        man.m_body1->getGameObject().getComponent<PlayerMoveComponent>()->setMoving(false);
    }

    if ((man.m_body2->getGameObject().getTag() == GameObjectTag::Player) &&
        (man.m_body1->getGameObject().getTag() == GameObjectTag::Platform) &&
        !man.m_body2->getGameObject().getComponent<PlayerMoveComponent>()->getJumpTimerActive())
    {
        man.m_body2->getGameObject().getComponent<PlayerMoveComponent>()->setGrounded(true);
        man.m_body2->getGameObject().getComponent<PlayerMoveComponent>()->setMoving(false);
    }             
}


std::unordered_map<bool, std::shared_ptr<mmt_gd::BoxCollisionComponent>>& mmt_gd::RigidBodyComponent::getColliders()
{
    return m_colliders;
}