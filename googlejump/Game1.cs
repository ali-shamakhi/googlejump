using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;



namespace googlejump
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        public static int i;

        // game properties
        public const bool test = true;
        public static int screen_width;
        public static int screen_height;
        public static bool gameOver = false;
        public static bool paused = false;
        public static bool fallen = false;
        public static bool menu = true;
        public static bool options = false;
        public static bool set_sound = true;
        public static float level_height;
        public static float height_per_score;
        public static Vector2 acceleration = new Vector2(0, -0.4f);
        public static Vector2 platform_size = new Vector2(80.0f, 20.0f);
        public static Vector2 normal_monster_size = new Vector2(60.0f, 60.0f);
        public static Vector2 moving_monster_size = new Vector2(60.0f, 60.0f);
        public static Vector2 huge_monster_size = new Vector2(140.0f, 100.0f);
        public static Vector2 ball_size = new Vector2(24.0f, 24.0f);
        public static float ball_speed;
        public static float platform_speed = 2.0f;
        public static float monster_speed = 2.0f;
        public static float doodle_normal_jump_speed = 15.0f;
        public static float doodle_max_move_speed = 12.0f;
        public static float doodle_max_ascend_speed = doodle_normal_jump_speed;
        public static float doodle_max_ascend_height = (doodle_normal_jump_speed * doodle_normal_jump_speed) / (2 * absolute(acceleration.Y));
        public static float doodle_max_distance_from_below = doodle_max_ascend_height +  platform_size.Y + 5;
        public static float doodle_move_acceleration = 1.2f;
        public static Vector2 trunk_size = new Vector2(24.0f,12.0f);
        public static Vector2 eyes_size = new Vector2(12.0f, 5.0f);
        public static KeyboardState keyboard_state;
        public static MouseState mouse_state;
        public static Texture2D background_texture;
        public static Texture2D menu_texture;
        public static Texture2D options_texture;
        public static Texture2D game_over_texture;
        public static Texture2D pause_texture;
        public static Texture2D platform_breakable_left_texture;
        public static Texture2D platform_breakable_right_texture;
        public static Texture2D ball_texture;
        public static Texture2D huge_monster_texture;
        public static Texture2D normal_monster_texture;
        public static Texture2D moving_monster_texture;
        public static Texture2D jump_pad_texture;
        public static Texture2D spring_texture;
        public static Texture2D doodle_texture;
        public static Texture2D trunk_texture;
        public static Texture2D eyes_texture;
        public static Texture2D platform_normal_texture;
        public static Texture2D platform_breakable_texture;
        public static Texture2D block_layer;
        public static Texture2D crosshair_texture;
        public static Texture2D tick_texture;
        public static SpriteFont drawing_font_title;
        public static SpriteFont drawing_font_text;
        // randomizer
        public static Random rnd = new Random();
        public static int rand(int start, int end)
        {
            if (start == end)
                return 0;
            else
                return (rnd.Next(start, end) * rnd.Next(start, end) * rnd.Next(start, end) + rnd.Next(start, end)) % (end - start) + start;
        }
        // probability
        public const int prb_container = 1000;
        public static float prb_platform_breakable;
        public const float max_prb_platform_breakable = 400;
        public static float prb_per_height_platform_breakable;
        public static float prb_platform_one_jump;
        public const float max_prb_platform_one_jump = 200;
        public static float prb_per_height_platform_one_jump;
        public static float prb_platform_moving;
        public const float max_prb_platform_moving = 200;
        public static float prb_per_height_platform_moving;

        public static float prb_monster_normal;
        public const float max_prb_monster_normal = 25;
        public static float prb_per_height_monster_normal;
        public static float prb_monster_moving;
        public const float max_prb_monster_moving = 25;
        public static float prb_per_height_monster_moving;
        public static float prb_monster_huge;
        public const float max_prb_monster_huge = 10;
        public static float prb_per_height_monster_huge;

        public static float min_space_platforms;
        public static float max_space_platforms;
        public static float spc_per_height_platforms;

        public static float limit_platform_breakable()
        {
            return prb_platform_breakable;
        }
        public static float limit_platform_one_jump()
        {
            return limit_platform_breakable() + prb_platform_one_jump;
        }
        public static float limit_platform_moving()
        {
            return limit_platform_one_jump() + prb_platform_moving;
        }

        public static float limit_monster_normal()
        {
            return prb_monster_normal;
        }
        public static float limit_monster_moving()
        {
            return limit_monster_normal() + prb_monster_moving;
        }
        public static float limit_monster_huge()
        {
            return limit_monster_moving() + prb_monster_huge;
        }
        // sounds
        public static SoundEffect bounce_sound;
        public static SoundEffect jump_pad_sound;
        public static SoundEffect game_over_sound;
        public static SoundEffect ball_sound;
        public static SoundEffect spring_sound;
        public static SoundEffect break_sound;
        public static SoundEffectInstance s_bounce;
        public static SoundEffectInstance s_jump_pad;
        public static SoundEffectInstance s_game_over;
        public static SoundEffectInstance s_ball;
        public static SoundEffectInstance s_spring;
        public static SoundEffectInstance s_break;
        // types
        public enum platform_type { NORMAL = 0, BREAKABLE = 1, ONE_JUMP = 2, MOVING = 3, BURNING = 4 };
        public enum enemy_type { NORMAL = 0, MOVING = 1, HUGE = 2, NONE = 3 };
        public enum powerup_type { SPRING = 0, SPRING_SHOE = 1, JUMP_PAD = 2, ROCKET = 3 };

        // cartesian y
        public static float cart_y(float y)
        {
            return (float)screen_height - y;
        }
        // abs
        public static float absolute(float n)
        {
            return ((n < 0) ? -n : n);
        }
        public static float vector_y(float size, Vector2 vect)
        {
            float vect_size = (float)Math.Sqrt(vect.X * vect.X + vect.Y * vect.Y);
            return vect.Y * (size / vect_size);
        }
        public static float vector_x(float size, Vector2 vect)
        {
            float vect_size = (float)Math.Sqrt(vect.X * vect.X + vect.Y * vect.Y);
            return vect.X * (size / vect_size);
        }
        // game initializer
        public void game_initialize()
        {
            gameOver = false;
            paused = false;
            menu = false;
            fallen = false;
            options = false;
            game_stats.just_escaped = false;

            level_height = 0.0f;

            prb_platform_breakable = 0;
            prb_platform_one_jump = 0;
            prb_platform_moving = 0;

            prb_monster_normal = 0;
            prb_monster_moving = 0;
            prb_monster_huge = 0;

            min_space_platforms = 15;
            max_space_platforms = 40;

            doodle.origin.X = screen_width / 2;
            doodle.origin.Y = 10;
            doodle.size.X = 40;
            doodle.size.Y = 55;
            doodle.velocity.X = 0;
            doodle.velocity.Y = doodle_normal_jump_speed;
            doodle.moving_right = true;
            doodle.freezed = false;

            trunk.direction = new Vector2(0, 1);

            // construct initial platforms
            platform_constructor(ref platform, true);
        }
        // make the last platform reachable
        public static void make_reachable(ref pltfrm new_plt, ref List<pltfrm> plt)
        {
            int end = plt.Count - 1;
            int start;
            for (start = end; start >= 0 && new_plt.origin.Y - plt[start].origin.Y < doodle_max_ascend_height - 10; start--)
                if (plt[start].type != platform_type.BREAKABLE)
                    return;
            int change_id = rand(start + 1, end + 1);
            int change = rand(0, 3);

            pltfrm[] plt_array = plt.ToArray();
            plt_array[change_id].type = (change == 0 ? platform_type.NORMAL :
                                   (change == 1 ? platform_type.ONE_JUMP :
                                    platform_type.MOVING));
            plt.Clear();
            plt = plt_array.ToList();
        }
        // platform constructor
        public static void platform_constructor(ref List<pltfrm> plt, bool init)
        {
            if (init)
            {
                i = 0;
                do
                {
                    new_platform = new pltfrm();
                    new_platform.origin.X = rnd.Next((int)(platform_size.X / 2), screen_width - (int)(platform_size.X / 2) + 1);
                    new_platform.origin.Y = (i == 0 ? (float)rnd.Next((int)min_space_platforms, (int)max_space_platforms + 1) :
                                            plt[i - 1].origin.Y + platform_size.Y + rnd.Next((int)min_space_platforms, (int)max_space_platforms + 1));
                    new_type = rand(0, prb_container);
                    new_platform.type = (new_type < limit_platform_breakable() ? platform_type.BREAKABLE :
                                        (new_type < limit_platform_one_jump() ? platform_type.ONE_JUMP :
                                        (new_type < limit_platform_moving() ? platform_type.MOVING :
                                        platform_type.NORMAL)));
                    if (new_platform.type == platform_type.MOVING)
                        new_platform.moving_right = (rnd.Next(2) == 1 ? true : false);

                    plt.Add(new_platform);

                    i++;
                } while (plt[i - 1].origin.Y < 2 * screen_height + level_height);
            }
            else
            {
                while (plt[plt.Count - 1].origin.Y < 2 * screen_height + level_height)
                {
                    new_platform = new pltfrm();
                    new_platform.origin.X = rnd.Next((int)(platform_size.X / 2), screen_width - (int)(platform_size.X / 2) + 1);
                    new_platform.origin.Y = plt[plt.Count - 1].origin.Y + platform_size.Y + rnd.Next((int)min_space_platforms, (int)max_space_platforms + 1);
                    new_type = rand(0, prb_container);
                    new_platform.type = (new_type < limit_platform_breakable() ? platform_type.BREAKABLE :
                                        (new_type < limit_platform_one_jump() ? platform_type.ONE_JUMP :
                                        (new_type < limit_platform_moving() ? platform_type.MOVING :
                                        platform_type.NORMAL)));
                    if (new_platform.type == platform_type.MOVING)
                        new_platform.moving_right = (rnd.Next(2) == 1 ? true : false);

                    make_reachable(ref new_platform, ref plt);
                    plt.Add(new_platform);
                }
            }
        }
        // enemy constructor
        public static void enemy_constructor(ref List<enmy> enm, ref List<pltfrm> plt)
        {
            if (enm.Count == 0)
            {
                new_enemy = new enmy();
                new_type = rand(0, prb_container);
                new_enemy.type = (new_type < limit_monster_normal() ? enemy_type.NORMAL :
                                    (new_type < limit_monster_moving() ? enemy_type.MOVING :
                                    (new_type < limit_monster_huge() ? enemy_type.HUGE :
                                    enemy_type.NONE)));
                if (new_enemy.type == enemy_type.NONE)
                    return;

                new_enemy.health = 1;
                if (new_enemy.type == enemy_type.HUGE)
                    new_enemy.health = 3;

                if (new_enemy.type == enemy_type.MOVING)
                    new_enemy.moving_right = (rnd.Next(2) == 1 ? true : false);

                Vector2 size = (new_enemy.type == enemy_type.NORMAL ? normal_monster_size :
                    (new_enemy.type == enemy_type.MOVING ? moving_monster_size :
                    (new_enemy.type == enemy_type.HUGE ? huge_monster_size : new Vector2(0, 0))));
                new_enemy.size = size;

                new_enemy.origin.X = rnd.Next((int)(size.X / 2), screen_width - (int)(size.X / 2) + 1);
                new_enemy.origin.Y = plt[plt.Count - 1].origin.Y + platform_size.Y + 10;

                enm.Add(new_enemy);
            }
            while (enm[enm.Count - 1].origin.Y < 2 * screen_height + level_height)
            {
                new_enemy = new enmy();
                new_type = rand(0, prb_container);
                new_enemy.type = (new_type < limit_monster_normal() ? enemy_type.NORMAL :
                                    (new_type < limit_monster_moving() ? enemy_type.MOVING :
                                    (new_type < limit_monster_huge() ? enemy_type.HUGE :
                                    enemy_type.NONE)));
                if (new_enemy.type == enemy_type.NONE)
                    return;

                new_enemy.health = 1;
                if (new_enemy.type == enemy_type.HUGE)
                    new_enemy.health = 3;

                if (new_enemy.type == enemy_type.MOVING)
                    new_enemy.moving_right = (rnd.Next(2) == 1 ? true : false);

                Vector2 size = (new_enemy.type == enemy_type.NORMAL ? normal_monster_size :
                    (new_enemy.type == enemy_type.MOVING ? moving_monster_size :
                    (new_enemy.type == enemy_type.HUGE ? huge_monster_size : new Vector2(0,0))));
                new_enemy.size = size;

                new_enemy.origin.X = rnd.Next((int)(size.X / 2), screen_width - (int)(size.X / 2) + 1);
                new_enemy.origin.Y = plt[plt.Count-1].origin.Y + platform_size.Y + 10;
                
                enm.Add(new_enemy);
            }
        }
        // ball constructor
        public static void ball_constructor(ref List<bll> bal)
        {
            if (trunk.heat <= 0 && !game_stats.just_played)
            {
                float shoot_origin_y = (doodle.origin.Y - level_height) + doodle.size.Y - 10;
                Vector2 direction = new Vector2(crosshair.origin.X - doodle.origin.X , crosshair.origin.Y - shoot_origin_y);
                new_ball = new bll();
                new_ball.origin.X = doodle.origin.X + vector_x(30, direction);
                new_ball.origin.Y = shoot_origin_y + vector_y(30, direction) + level_height;
                new_ball.velocity.X = vector_x(ball_speed, direction);
                new_ball.velocity.Y = vector_y(ball_speed, direction);
                new_ball.rotation = 0;
                new_ball.rotation_speed = rnd.Next((int)(-Math.PI / 6), (int)(Math.PI / 6));
                bal.Add(new_ball);

                if (set_sound) s_ball.Play();

                trunk.direction = direction;
                trunk.heat = 2.0f;
            }

        }
        public static void destroy(ref List<pltfrm> plt, ref List<bll> bal,ref List<enmy> enm, bool all)
        {
            if (all)
            {
                for (i = 0; i < plt.Count; i++)
                {
                     plt.RemoveAt(i);
                     i--;
                }
                for (i = 0; i < bal.Count; i++)
                {
                    bal.RemoveAt(i);
                    i--;
                }
                for (i = 0; i < enm.Count; i++)
                {
                    enm.RemoveAt(i);
                    i--;
                }
            }
            else
            {
                for (i = 0; i < plt.Count; i++)
                {
                    if (plt[i].origin.Y + platform_size.Y / 2 < level_height)
                    {
                        plt.RemoveAt(i);
                        i--;
                    }
                }
                for (i = 0; i < bal.Count; i++)
                {
                    if (bal[i].origin.Y + ball_size.Y/2 < level_height || bal[i].origin.X + ball_size.X/2 < 0 || bal[i].origin.X - ball_size.X/2 > screen_width )
                    {
                        bal.RemoveAt(i);
                        i--;
                    }
                }
                for (i = 0; i < enm.Count; i++)
                {
                    if (enm[i].origin.Y + enm[i].size.Y < level_height)
                    {
                        enm.RemoveAt(i);
                        i--;
                    }
                }
            }
        }

        // game stats
        public struct gm_stts
        {
            public Vector2 title;
            public bool escaped;
            public bool just_escaped;
            public bool just_played;
            public bool button_play()
            {
                return (mouse_state.X >= 270 && mouse_state.Y >= 500 && mouse_state.X <= 400 && mouse_state.Y <= 535);
            }
            public bool button_options_in_menu()
            {
                return (mouse_state.X >= 180 && mouse_state.Y >= 555 && mouse_state.X <= 310 && mouse_state.Y <= 590);
            }
            public bool button_play_again()
            {
                return (mouse_state.X >= 165 && mouse_state.Y >= 436 && mouse_state.X <= 340 && mouse_state.Y <= 490);
            }
            public bool button_menu_in_game_over()
            {
                return (mouse_state.X >= 260 && mouse_state.Y >= 530 && mouse_state.X <= 435 && mouse_state.Y <= 580);
            }
            public bool button_resume()
            {
                return (mouse_state.X >= 305 && mouse_state.Y >= 100 && mouse_state.X <= 470 && mouse_state.Y <= 145);
            }
            public bool button_options_in_pause()
            {
                return (mouse_state.X >= 300 && mouse_state.Y >= 580 && mouse_state.X <= 465 && mouse_state.Y <= 630);
            }
            public bool button_menu_in_pause()
            {
                return (mouse_state.X >= 10 && mouse_state.Y >= 580 && mouse_state.X <= 175 && mouse_state.Y <= 630);
            }
            public bool button_set_sound()
            {
                return (mouse_state.X >= 224 && mouse_state.Y >= 239 && mouse_state.X <= 242 && mouse_state.Y <= 254);
            }
            public bool button_exit()
            {
                return (mouse_state.X >= 40 && mouse_state.Y >= 590 && mouse_state.X <= 165 && mouse_state.Y <= 615);
            }
        }
        public static void update_prb()
        {
            if (prb_platform_breakable < max_prb_platform_breakable)
                prb_platform_breakable += prb_per_height_platform_breakable;
            if (prb_platform_one_jump < max_prb_platform_one_jump)
                prb_platform_one_jump += prb_per_height_platform_one_jump;
            if (prb_platform_moving < max_prb_platform_moving)
                prb_platform_moving += prb_per_height_platform_moving;

            if (min_space_platforms < doodle_max_ascend_height * 0.8f)
                min_space_platforms += spc_per_height_platforms;
            if (max_space_platforms < doodle_max_ascend_height - 10)
                max_space_platforms += spc_per_height_platforms;

            if (prb_monster_normal < max_prb_monster_normal)
                prb_monster_normal += prb_per_height_monster_normal;
            if (prb_monster_moving < max_prb_monster_moving)
                prb_monster_moving += prb_per_height_monster_moving;
            if (prb_monster_huge < max_prb_monster_huge)
                prb_monster_huge += prb_per_height_monster_huge;
        }

        // mouse
        public struct ms
        {
            public Vector2 origin;
            public Vector2 size;
            public bool clicked;
            public bool just_clicked;
            public void update()
            {
                origin.X = mouse_state.X;
                origin.Y = cart_y(mouse_state.Y);
            }
            public Rectangle drawing_boundary()
            {
                return new Rectangle((int)(origin.X - size.X / 2), (int)cart_y(origin.Y + size.Y / 2), (int)(size.X), (int)(size.Y));
            }
            public bool in_screen()
            {
                return (origin.X >= 0 && origin.Y >= 0 && origin.X < screen_width && origin.Y < screen_height);
            }
            public void draw(SpriteBatch spr)
            {
                if (in_screen())
                    spr.Draw(crosshair_texture, drawing_boundary() , Color.White);
            }
        }

        // doodle
        public struct ddl
        {
            public Vector2 p_origin;
            public Vector2 origin;
            public Vector2 size;
            public Vector2 velocity;
            public bool moving_right;
            public bool freezed;
            public void update()
            {
                p_origin.X = origin.X;
                p_origin.Y = origin.Y;

                origin.Y += (velocity.Y < doodle_max_ascend_speed ? velocity.Y : doodle_max_ascend_speed);
                // check speed limits
                if (velocity.Y < doodle_max_ascend_speed || acceleration.Y < 0.0f)
                    velocity.Y += acceleration.Y;

                if (!freezed)
                {
                    
                    origin.X += (absolute(velocity.X) < doodle_max_move_speed ? velocity.X : doodle_max_move_speed * (velocity.X / absolute(velocity.X)));
                    
                    // check speed limits
                    if (absolute(velocity.X) < doodle_max_move_speed || velocity.X * acceleration.X < 0.0f)
                        velocity.X += acceleration.X;
                    // update facing
                    if (velocity.X > 0.0f)
                        moving_right = true;
                    else if (velocity.X < 0.0f)
                        moving_right = false;

                    // no vibration
                    if (absolute(velocity.X) < 0.1f)
                        velocity.X = 0;
                    if (absolute(velocity.X) < 0.1f)
                        velocity.X = 0;
                    // get back to screen
                    if (origin.X > screen_width)
                        origin.X -= screen_width;
                    if (origin.X < 0)
                        origin.X += screen_width;
                }
            }
            public Rectangle boundary()
            {
                return new Rectangle((int)(origin.X - size.X / 2), (int)(origin.Y - level_height), (int)(size.X), (int)(size.Y));
            }
            public Rectangle drawing_boundary()
            {
                return new Rectangle((int)(origin.X - size.X / 2), (int)(cart_y(origin.Y - level_height) - size.Y), (int)(size.X), (int)(size.Y));
            }
            public bool collision(Rectangle target)
            {
                Rectangle bndry = boundary();
                if (bndry.X + bndry.Width > target.X && bndry.X < target.X + target.Width
                    && bndry.Y + bndry.Height > target.Y && bndry.Y < target.Y + target.Height)
                    return true;
                else
                    return false;
            }
            public bool touch(Rectangle target)
            {
                Rectangle bndry = boundary();
                if (bndry.X + bndry.Width > target.X && bndry.X < target.X + target.Width
                    && ((int)(p_origin.Y - level_height) >= target.Y + target.Height) && bndry.Y <= target.Y + target.Height)
                    return true;
                else
                    return false;
            }
            public bool landed(Rectangle target)
            {
                if (velocity.Y <= 0)
                    return touch(target);
                else
                    return false;
            }
            public void draw(SpriteBatch spr)
            {
                spr.Draw(doodle_texture, drawing_boundary(), null, Color.White,0,new Vector2(0,0),(moving_right ? SpriteEffects.None : SpriteEffects.FlipHorizontally ),0);
            }
        }
        // trunk
        public struct trnk
        {
            public Vector2 direction;
            public float heat;
            public float angle;
            public void update()
            {
                if (heat >= 0.0f)
                    heat -= 0.1f;
                angle = -(direction.X == 0 ? (float)Math.PI / 2.0f : (float)Math.Atan2(direction.Y, direction.X));
            }
            public void draw(SpriteBatch spr)
            {
                spr.Draw(trunk_texture,new Rectangle((int)(doodle.origin.X),(int)cart_y(doodle.origin.Y + doodle.size.Y - 10 - level_height),(int)trunk_size.X,(int)trunk_size.Y),
                    null,Color.White,(heat > 0.0f ? (doodle.moving_right ? angle : angle + (float)Math.PI ) : 0.0f),(doodle.moving_right ? new Vector2(-15,trunk_size.Y/2) : new Vector2(trunk_size.X + 32, trunk_size.Y/2)),
                    (doodle.moving_right ? SpriteEffects.None : SpriteEffects.FlipHorizontally),0);
                float x_offset = (heat > 0.0f ? vector_x(5, direction) - 4 : (doodle.moving_right ? 0 : -4));
                float y_offset = (heat > 0.0f ? vector_y(5, direction) : 0);
                spr.Draw(eyes_texture, new Rectangle((int)(doodle.origin.X + x_offset), (int)cart_y(doodle.origin.Y + doodle.size.Y - 10 - level_height + y_offset), (int)eyes_size.X, (int)eyes_size.Y),
                    null, Color.White, 0.0f, new Vector2(eyes_size.X / 2, eyes_size.Y / 2),
                    (doodle.moving_right ? SpriteEffects.None : SpriteEffects.FlipHorizontally), 0);
            }

        }
        // ball
        public struct bll
        {
            public Vector2 origin;
            public Vector2 velocity;
            public float rotation;
            public float rotation_speed;
            public void update()
            {
                origin.X += velocity.X;
                origin.Y += velocity.Y;
                velocity.Y += acceleration.Y;
                rotation += rotation_speed;
            }
            public Rectangle boundary()
            {
                return new Rectangle((int)(origin.X - ball_size.X / 2), (int)(origin.Y - ball_size.Y / 2 - level_height), (int)(ball_size.X), (int)(ball_size.Y));
            }
            public Rectangle drawing_boundary()
            {
                return new Rectangle((int)(origin.X - ball_size.X / 2), (int)(cart_y(origin.Y + ball_size.Y / 2 - level_height)), (int)(ball_size.X), (int)(ball_size.Y));
            }
            public bool in_screen()
            {
                return (origin.X >= -ball_size.X / 2 && origin.Y >= -ball_size.Y / 2 && origin.X < screen_width + ball_size.X / 2 && origin.Y < screen_height + ball_size.Y);
            }
            public bool collision(Rectangle target)
            {
                Rectangle bndry = boundary();
                if (bndry.X + bndry.Width > target.X && bndry.X < target.X + target.Width
                    && bndry.Y + bndry.Height > target.Y && bndry.Y < target.Y + target.Height)
                    return true;
                else
                    return false;
            }
            public void draw(SpriteBatch spr)
            {
                spr.Draw(ball_texture, drawing_boundary(), null, Color.White, rotation, new Vector2(ball_size.X/2,ball_size.Y/2), SpriteEffects.None, 0);
            }
        }
        // platform
        public struct pltfrm
        {
            public platform_type type;
            public Vector2 origin;
            public bool moving_right;
            public Rectangle boundary()
            {
                return new Rectangle((int)(origin.X - platform_size.X / 2), (int)(origin.Y - level_height), (int)(platform_size.X), (int)(platform_size.Y));
            }
            public Rectangle drawing_boundary()
            {
                return new Rectangle((int)(origin.X - platform_size.X / 2), (int)(cart_y(origin.Y - level_height) - platform_size.Y), (int)(platform_size.X), (int)(platform_size.Y));
            }
            public void update()
            {
                if (moving_right)
                        origin.X += platform_speed;
                    else
                        origin.X -= platform_speed;
                    if ((origin.X > screen_width - platform_size.X / 2 && moving_right) || (origin.X < platform_size.X / 2 && !moving_right))
                        moving_right = (moving_right ? false : true);
            }
            public void draw(SpriteBatch spr)
            {
                spr.Draw((type == platform_type.BREAKABLE ? platform_breakable_texture : platform_normal_texture), drawing_boundary(), null, (type == platform_type.ONE_JUMP ? Color.LightBlue :  Color.White));
            }
        }
        // enemy
        public struct enmy
        {
            public enemy_type type;
            public Vector2 origin;
            public Vector2 size;
            public byte health;
            public bool moving_right;
            public Rectangle boundary()
            {
                return new Rectangle((int)(origin.X - size.X / 2), (int)(origin.Y - level_height), (int)(size.X), (int)(size.Y));
            }
            public Rectangle drawing_boundary()
            {
                return new Rectangle((int)(origin.X - size.X / 2), (int)cart_y(origin.Y + size.Y - level_height), (int)(size.X), (int)(size.Y));
            }
            public void update()
            {
                if (moving_right)
                    origin.X += monster_speed;
                else
                    origin.X -= monster_speed;
                if ((origin.X > screen_width - size.X / 2 && moving_right) || (origin.X < size.X / 2 && !moving_right))
                    moving_right = (moving_right ? false : true);
            }
            public void draw(SpriteBatch spr)
            {
                switch (type)
                {
                    case enemy_type.NORMAL: spr.Draw(normal_monster_texture, drawing_boundary(), Color.White);
                        break;
                    case enemy_type.HUGE: spr.Draw(huge_monster_texture, drawing_boundary(), Color.White);
                        break;
                    case enemy_type.MOVING: spr.Draw(moving_monster_texture, drawing_boundary(), Color.White);
                        break;
                }
            }
        }

        // instances of objects
        public static gm_stts game_stats;
        public static ms crosshair;
        public static ddl doodle;
        public static trnk trunk;
        public static pltfrm new_platform;
        public static enmy new_enemy;
        public static bll new_ball;
        public static int new_type;
        public List<pltfrm> platform = new List<pltfrm>();
        public List<enmy> enemy = new List<enmy>();
        public List<bll> ball = new List<bll>();
        
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.IsFullScreen = false;
            graphics.PreferredBackBufferHeight = 640;
            graphics.PreferredBackBufferWidth = 480;
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            // game properties
            screen_width = Window.ClientBounds.Width;
            screen_height = Window.ClientBounds.Height;
            height_per_score = 5.0f;
            prb_per_height_platform_breakable = 1.0f / (0.05f * screen_height);
            prb_per_height_platform_one_jump = 1.0f / (0.03f * screen_height);
            prb_per_height_platform_moving = 1.0f / (0.01f * screen_height);
            spc_per_height_platforms = 1.0f / (0.25f * screen_height);
            prb_per_height_monster_normal = 1.0f / (2.0f * screen_height);
            prb_per_height_monster_moving = 1.0f / (2.0f * screen_height);
            prb_per_height_monster_huge = 1.0f / (5.0f * screen_height);
            crosshair.size.X = 32;
            crosshair.size.Y = 32;
            crosshair.just_clicked = false;
            game_stats.escaped = false;
            game_stats.just_escaped = false;
            game_stats.just_played = false;
            ball_speed = 10.0f;
            // game_stats.title = new Vector2(50,500);

            // game_initialize();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // loading textures
            background_texture = Content.Load<Texture2D>("background");
            menu_texture = Content.Load <Texture2D> ("menu");
            options_texture = Content.Load <Texture2D> ("options");
            game_over_texture = Content.Load<Texture2D>("game_over");
            pause_texture = Content.Load <Texture2D> ("pause");
            doodle_texture = Content.Load<Texture2D>("doodle");
            trunk_texture = Content.Load<Texture2D>("trunk");
            eyes_texture = Content.Load<Texture2D>("eyes");
            platform_normal_texture = Content.Load<Texture2D>("platform_normal");
            platform_breakable_texture = Content.Load<Texture2D>("platform_breakable");
            platform_breakable_left_texture = Content.Load<Texture2D>("brL");
            platform_breakable_right_texture = Content.Load<Texture2D>("brR");
            ball_texture = Content.Load<Texture2D>("ball");
            huge_monster_texture = Content.Load<Texture2D>("huge_monster");
            normal_monster_texture = Content.Load<Texture2D>("normal_monster");
            moving_monster_texture = Content.Load<Texture2D>("normal_monster");
            spring_texture = Content.Load<Texture2D>("spring");
            jump_pad_texture = Content.Load<Texture2D>("jump_pad");
            block_layer = Content.Load<Texture2D>("block");
            crosshair_texture = Content.Load<Texture2D>("crosshair");
            tick_texture = Content.Load<Texture2D>("tick");

            drawing_font_title = Content.Load<SpriteFont>("al-seana-title");
            drawing_font_text = Content.Load<SpriteFont>("al-seana-text");

            bounce_sound = Content.Load<SoundEffect>("bounce_sound");
            ball_sound = Content.Load<SoundEffect>("ball_sound");
            break_sound = Content.Load<SoundEffect>("break_sound");
            spring_sound = Content.Load<SoundEffect>("spring_sound");
            game_over_sound = Content.Load<SoundEffect>("game_over_sound");

            s_bounce = bounce_sound.CreateInstance();
            s_ball = ball_sound.CreateInstance();
            s_break = break_sound.CreateInstance();
            s_spring = spring_sound.CreateInstance();
            s_game_over = game_over_sound.CreateInstance();

            // TODO: use this.Content to load your game content here
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // start update
            keyboard_state = Keyboard.GetState();
            mouse_state = Mouse.GetState();

            // mouse control
            crosshair.update();
            if (mouse_state.LeftButton == ButtonState.Pressed)
            {
                if (!crosshair.just_clicked)
                {
                    crosshair.clicked = true;
                    crosshair.just_clicked = true;
                }
            }
            else
            {
                crosshair.just_clicked = false;
                game_stats.just_played = false;
            }
            // escape control
            if (keyboard_state.IsKeyDown(Keys.Escape))
            {
                if (!game_stats.just_escaped)
                {
                    game_stats.escaped = true;
                    game_stats.just_escaped = true;
                }
            }
            else
                game_stats.just_escaped = false;
            // different game states
            if (options)
            {
                if (game_stats.escaped)
                    options = false;
                else if (crosshair.clicked && game_stats.button_set_sound())
                    set_sound = (set_sound ? false : true);
            }
            else if (menu)
            {
                if (crosshair.clicked)
                {
                    if (game_stats.button_exit())
                    {
                        this.Exit();
                    }
                    if (game_stats.button_play())
                    {
                        gameOver = false;
                        paused = false;
                        game_initialize();
                        game_stats.just_played = true;
                    }
                    else if (game_stats.button_options_in_menu())
                    {
                        options = true;
                    }
                }

            }
            // game flow : main update
            else if (!gameOver)
            {
                if (fallen)
                {
                    if (crosshair.clicked)
                    {
                        if (game_stats.button_play_again())
                        {
                            gameOver = false;
                            paused = false;
                            game_initialize();
                            game_stats.just_played = true;
                            fallen = false;
                        }
                        else if (game_stats.button_menu_in_game_over())
                        {
                            menu = true;
                        }
                    }
                }
                else if (!paused)
                {
                    // update position, ...
                    doodle.update();
                    trunk.update();
                    pltfrm[] platform_array = platform.ToArray();
                    for (i = 0; i < platform.Count; i++)
                    {
                        if (platform_array[i].type == platform_type.MOVING)
                            platform_array[i].update();
                    }
                    platform.Clear();
                    platform = platform_array.ToList();

                    enmy[] enemy_array = enemy.ToArray();
                    for (i = 0; i < enemy.Count; i++)
                    {
                        if (enemy_array[i].type == enemy_type.MOVING)
                            enemy_array[i].update();
                    }
                    enemy.Clear();
                    enemy = enemy_array.ToList();

                    bll[] ball_array = ball.ToArray();
                    for (i = 0; i < ball.Count; i++ )
                    {
                        ball_array[i].update();
                    }
                    ball.Clear();
                    ball = ball_array.ToList();

                    // move camera
                    if (level_height < doodle.origin.Y - doodle_max_distance_from_below)
                        level_height = doodle.origin.Y - doodle_max_distance_from_below;
                    // constructions
                    platform_constructor(ref platform, false);

                    enemy_constructor(ref enemy, ref platform);

                    // update probabilities
                    update_prb();

                    if (mouse_state.LeftButton == ButtonState.Pressed)
                        ball_constructor(ref ball);
                    
                    // platform collision
                    for (i = 0; i < platform.Count; i++)
                    {
                        // landing
                        if (doodle.landed(platform[i].boundary()) && !doodle.freezed)
                        {
                            if (platform[i].type != platform_type.BREAKABLE)
                            {
                                doodle.velocity.Y = doodle_normal_jump_speed;
                                if (set_sound) s_bounce.Play();
                                if (platform[i].type == platform_type.ONE_JUMP)
                                    platform.RemoveAt(i);
                            }
                            else
                            {
                                platform.RemoveAt(i);
                                if (set_sound) s_break.Play();
                            }
                        }
                    }
                    // enemy collision
                    for (i = 0; i < enemy.Count; i++)
                    {
                        if (doodle.collision(enemy[i].boundary()))
                        {
                            doodle.freezed = true;
                            if (doodle.velocity.Y < 0.0f)
                                doodle.velocity.Y = 0.0f;
                        }

                        int enemy_removed_count = 0;
                        for(int j = 0 ; j < ball.Count ; j++)
                        {
                            if (i < 0)
                                i = 0;
                            if (enemy.Count == 0)
                                break;
                            if (ball[j].collision(enemy[i].boundary()))
                            {
                                enmy[] enemy_array2 = enemy.ToArray();
                                enemy_array2[i].health--;
                                enemy.Clear();
                                enemy = enemy_array2.ToList();

                                if (enemy[i].health <= 0)
                                {
                                    enemy.RemoveAt(i);
                                    enemy_removed_count++;
                                }
                                ball.RemoveAt(j);
                                j--;
                            }
                            i -= enemy_removed_count;
                            enemy_removed_count = 0;
                        }
                    }

                    if (!fallen && doodle.origin.Y < level_height)
                    {
                        fallen = true;
                        destroy(ref platform, ref ball, ref enemy, true);
                        if (set_sound) s_game_over.Play();
                    }
                }
                else if (paused)
                {
                    if (crosshair.clicked)
                    {
                        if (game_stats.button_resume())
                        {
                            paused = false;
                            game_stats.just_played = true;
                        }
                        else if (game_stats.button_menu_in_pause())
                        {
                            menu = true;
                            destroy(ref platform, ref ball, ref enemy, true);
                        }
                        else if (game_stats.button_options_in_pause())
                            options = true;
                    }
                }
                
                // arrow controls
                if (keyboard_state.IsKeyDown(Keys.Right) == keyboard_state.IsKeyDown(Keys.Left))
                {
                    if (doodle.velocity.X > 0)
                        acceleration.X = -doodle_move_acceleration;
                    else if (doodle.velocity.X < 0)
                        acceleration.X = +doodle_move_acceleration;
                    else
                        acceleration.X = 0.0f;
                }
                else if (keyboard_state.IsKeyDown(Keys.Right))
                    acceleration.X = +doodle_move_acceleration;
                else if (keyboard_state.IsKeyDown(Keys.Left))
                    acceleration.X = -doodle_move_acceleration;

                if (game_stats.escaped)
                {
                    paused = (paused ? false : true);
                }
            }
            
            // end update

            // destroy
            destroy(ref platform, ref ball, ref enemy, false);

            crosshair.clicked = false;
            game_stats.escaped = false;

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);
            spriteBatch.Begin();
            // TODO: Add your drawing code here
            if (options)
            {
                spriteBatch.Draw(options_texture, new Rectangle(0, 0, screen_width, screen_height), Color.White);
                if (set_sound)
                    spriteBatch.Draw(tick_texture,new Rectangle(233-15/2,247-19/2,36/2,36/2),Color.White );
            }
            else if (menu)
            {
                spriteBatch.Draw(menu_texture, new Rectangle(0, 0, screen_width, screen_height), Color.White);
            }
            else if (!gameOver)
            {
                if (fallen)
                {
                    spriteBatch.Draw(game_over_texture, new Rectangle(0, 0, screen_width, screen_height), Color.White);
                    spriteBatch.DrawString(drawing_font_text, ((int)level_height).ToString(), new Vector2(325, 230), Color.Black);
                }
                else
                {
                    spriteBatch.Draw(background_texture, new Rectangle(0, 0, screen_width, screen_height), Color.White);

                    for (i = 0; i < platform.Count; i++)
                        platform[i].draw(spriteBatch);

                    for (i = 0; i < enemy.Count; i++)
                        enemy[i].draw(spriteBatch);

                    for (i = 0; i < ball.Count; i++)
                        ball[i].draw(spriteBatch);

                    doodle.draw(spriteBatch);
                    trunk.draw(spriteBatch);

                    if (paused)
                    {
                        spriteBatch.Draw(block_layer, new Rectangle(0, 0, screen_width, screen_height), Color.White);
                        spriteBatch.Draw(pause_texture, new Rectangle(0, 0, screen_width, screen_height), Color.White);
                    }
                    spriteBatch.DrawString(drawing_font_text, "score: " + ((int)(level_height / height_per_score)).ToString(), new Vector2(20, 20), Color.Black);
                }
            }
            // mouse
            crosshair.draw(spriteBatch);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
