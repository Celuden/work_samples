/*
studies: MultiMediaTechnology / FHS
cause: MultiMediaProjekt 2a
team: Alija Suljic, Sabine Steiner, Markus Rinnerthaler
*/

#pragma once
#include "GameObject.hpp"
#include "BoxCollisionComponent.hpp"
#include "IComponent.hpp"

#include <unordered_map>

namespace mmt_gd
{
struct Manifold;

class RigidBodyComponent final : public IComponent
{
public:
    RigidBodyComponent(GameObject& gameObject, float mass);

    bool                                                              init() override;
    void                                                              update(float deltaTime) override;
    void                                                              onCollision(Manifold& man);
    std::unordered_map<bool, std::shared_ptr<BoxCollisionComponent>>& getColliders();
    sf::Vector2f&                                                     getPosition()
    {
        return m_position;
    }
    void setPosition(const sf::Vector2f position)
    {
        m_position = position;
        m_gameObject.setPosition(position);
    }

    sf::Vector2f m_acceleration;
    sf::Vector2f m_velocity;
    float        m_mass;
    float        m_inverseMass;
    std::list<sf::Vector2f> m_impulses;

private:
    std::unordered_map<bool, std::shared_ptr<BoxCollisionComponent>> m_colliders;
    sf::Vector2f                                                     m_position;
};
} // namespace mmt_gd
