// Impressum
// Das Projekt "Maro's Mayhem" ist im Studiengang MultiMediaTechnology / FHS im Rahmen des MultiMediaProjekt 1 von Alija Suljic erstellt worden.
// The project "Maro's Mayhem" has been developed within the MultiMediaTechnology Bachelor Studies at the Fachhochschule Salzburg as part of the MultiMediaProject 1 by Alija Suljic in the year 2022.

using SFML.Graphics;

internal abstract class GameObject : Transformable
{
    public abstract void Initialize();
    public abstract void Update(float deltaTime);
    public abstract void Draw(RenderWindow window);
}
