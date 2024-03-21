using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class EnemyBuffs
{
    public float healthMod, speedMod, dmgMod;
    public long pointsMod;

    public EnemyBuffs(float h, float s, float d, long p)
    {
        healthMod = h;
        speedMod = s;
        dmgMod = d;
        pointsMod = p;
    }

    public static EnemyBuffs Of(float h, float s, float d, long p)
    {
        return new EnemyBuffs(h, s, d, p);
    }
}
