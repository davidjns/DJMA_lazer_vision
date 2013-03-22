using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LazerEyez.GameModel.Game_Objects
{
    public class Health
    {
        private float life_percentage;
        private int no_of_life;

        public Health()
        {
            life_percentage = 100f;
            no_of_life = 1;
        }

        public Health(int init_no_of_life)
        {
            life_percentage = 100f;
            no_of_life = init_no_of_life;
        }

        public void resetHealth(int _no_of_life, float _life_percentage)
        {
            no_of_life = _no_of_life;
            life_percentage = _life_percentage;
        }

        public HealthStatus getStatus()
        {
            if (life_percentage >= 90) return HealthStatus.EXCELLENT;

            if (life_percentage >= 80) return HealthStatus.GOOD;

            if (life_percentage >= 60) return HealthStatus.BAD;

            if (life_percentage >= 30) return HealthStatus.LOW_BLOOD;

            if (life_percentage > 0) return HealthStatus.DANGER;

            return HealthStatus.DEAD;
        }

        public int addLife(int _no_of_life)
        {
            no_of_life += _no_of_life;
            return no_of_life;
        }

        public float addHealth(float health)
        {
            life_percentage += health;
            return life_percentage;
        }

        public float minusHealth(float health)
        {
            life_percentage -= health;
            if (life_percentage < 0)
            {
                life_percentage = 0;
                no_of_life -= 1;
            }
            return life_percentage;
        }
        public int getNoOfLife(){
            return no_of_life;
        }
        public float getHealth()
        {
            return life_percentage;
        }
    }

    public enum HealthStatus {EXCELLENT, GOOD, BAD, LOW_BLOOD, DANGER, DEAD };
}
