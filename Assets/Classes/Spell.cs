public class Spell
{
    static System.Random r = new System.Random();
    public string name;
    public int cost;

    static Spell[] spells = new Spell[]{
        new Spell("ghost",1)
    };

    public Spell(string name, int cost)
    {
        this.name = name;
        this.cost = cost;
    }

    public static Spell GetSpell()
    {
        return spells[r.Next(spells.Length)];
    }
}