﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using System.Text;

namespace Solar_Magic_Advance
{
    public static class LevelCard
    {
        //SMA4 Level Card Header
        public struct header
        {
            public byte eCoin;
            public byte acecoins;
            public byte lvl_class;
            public byte lvl_num;
            public byte lvl_icon;

            public string lvl_name;
        }

        //SMA4 eCoin data
        public struct eCoin
        {
            public byte[] pal;          //0x20 bytes
            public byte[] gfx;          //0x120 bytes
        }

        //SMA4 Room
        public struct room
        {
            public obj obj;
            public set set;
            public static List<warp> warps;
            public static List<spr> spr;
            public blkpth blkpth;
            public static List<auto_scroll> auto_scroll;
        }

        //SMA4 Object Header
        public struct obj
        {
            public UInt16 timeLimit;
            public UInt16 unk1;
            public byte unk1;           //4-bit
            public byte roomlength;
            public byte bgColor;
            public byte scrollSet;      //4-bit
            public byte unk2;           //4-bit
            public byte entryAction;    //3-bit
            public byte gfxset1;        //5-bit
            public byte gfxset2;        //4-bit
            public byte unk3;           //4-bit
            public byte extColor;       //4-bit
            public byte extEffect;      //4-bit
            public byte bgGFX;

            public static List<objd> data;
        }

        //SMA4 Object Data
        public struct objd
        {
            public byte bank;
            public byte length;
            public byte y;
            public byte x;
            public byte id;
            public byte slength;

            public void edit(byte bank, byte id, byte x, byte y, byte length)
            {
                this.bank = bank;
                this.id = id;
                this.x = x;
                this.y = y;
                this.length = length;
            }

            public void edit(byte bank, byte id, byte x, byte y, byte length, byte slength)
            {
                this.bank = bank;
                this.id = id;
                this.x = x;
                this.y = y;
                this.length = length;
                this.slength = slength;
            }
        }

        //SMA4 Room Settings
        public struct set
        {
            public UInt16 scr_y_bound;
            public UInt16 scr_centerYfix;
            public UInt16 scr_centerYply;
            public byte scr_distYmin;
            public byte scr_distYmax;
            public byte player_Y;
            public byte player_X;
            public byte screen_Y;
            public byte screen_X;
            public UInt16 objset;
            public UInt16 music;
            public byte gfxset1;
            public byte gfxset2;
            public byte gfxset3;
            public byte gfxset4;
            public byte gfxset5;
            public byte gfxset6;
            public UInt16 unk1;
            public UInt16 unk2;
            public byte unk3;
            public byte unk4;
            public byte unk5;
            public byte unk6;
            public byte unk7;
            public byte unk8;
        }

        //SMA4 Warp Data
        public struct warp
        {
            public byte srcX;
            public byte srcY;
            public byte roomdst;
            public byte unk1;
            public byte dstX;
            public byte dstY;
            public byte center_scrX;
            public byte center_scrY;
            public byte unk2;
            public byte exit_type;
        }

        //SMA4 Sprite
        public struct spr
        {        
	        public byte bank;
	        public byte id;
	        public byte x;
	        public byte y;
	        public byte p1;
            public byte p2;
	
	        public void edit(byte bank, byte id, byte x, byte y)
	        {
		        this.bank = bank;
		        this.id = id;
		        this.x = x;
		        this.y = y;
	        }
	
	        public void edit(byte bank, byte id, byte x, byte y, byte p1)
	        {
		        this.bank = bank;
		        this.id = id;
		        this.x = x;
		        this.y = y;
		        this.p1 = p1;
	        }
	
	        public void edit(byte bank, byte id, byte x, byte y, byte p1, byte p2)
	        {
		        this.bank = bank;
		        this.id = id;
		        this.x = x;
		        this.y = y;
		        this.p1 = p1;
		        this.p2 = p2;
	        }

            public byte size()
            {
                return getSpriteSize(this.bank, this.id);
            }
        }

        //SMA4 Block Path Header
        public struct blkpth
        {
            public bool init_direction;     //false = Right; true = Left
            public byte length;             //4-bit
            public byte speed;              //3-bit

            public static List<blkpthd> data;
        }

        //SMA4 Block Path Data
        public struct blkpthd
        {
            public byte dist_blocks;        //6-bit (63 MAX)
            public byte direction;          //2-bit (0 = Right, 1 = Left, 2 = Up, 3 = Down)
        }

        //SMA4 Auto-Scroll
        public struct auto_scroll
        {
            public byte x;
            public byte y;
            public byte speed;
        }

        //------------------------------------Get stuff
        public static string get_eCoin_position(byte position)
        {
            //8 e-Coins per floor, 3 floors total, 8*3 = 24
            if (position == 0)
                return "No";
            else if (position <= 24)
            {
                int floor = (int)Math.Ceiling((double)(position/8));
                int pos = position - ((floor-1)*8);
                return floor.ToString() + "F, " + getOrdinal(pos);
            }
            else
                return "Invalid";
        }

        public static string getOrdinal(int num)
        {
            if ((num > 3) && (num < 21))
                return num.ToString() + "th";
            else
            {
                switch (num % 10)
                {
                    case 1:
                        return num.ToString() + "st";
                    case 2:
                        return num.ToString() + "nd";
                    case 3:
                        return num.ToString() + "rd";
                    default:
                        return num.ToString() + "th";
                }
            }
        }

        public static string getLevelSet(byte lvl_set)
        {
            if (lvl_set < levelSet.Length)
                return levelSet[lvl_set];
            else
                return "Invalid";
        }

        public static string getLevelIcon(byte lvl_icon)
        {
            if (lvl_icon < levelIcon.Length)
                return levelIcon[lvl_icon];
            else
                return "Invalid";
        }

        public static byte getSpriteSize(byte bank, byte id)
        {
            int fullID = (int)id ^ ((int)(bank << 8));
            if (fullID < spriteSize.Length)
                return spriteSize[fullID];
            else if (fullID == 0x01AB)
                return 5;
            else
                return 4;
        }

        public static string getLevelName(byte[] lvl_name)
        {
            string name = "";
            for (int i = 0; i < lvl_name.Length; i++)
                name += USFont[lvl_name[i]];
            return name;
        }

        //---------------------------------Arrays
        public static string[] USFont =
        {
            // # = Invalid
            "A","B","C","D","E","F","G","H","I","J","K","L","M",
            "N","O","P","Q","R","S","T","U","V","W","X","Y","Z",
            "#","#","'",",",".","#",

            "a","b","c","d","e","f","g","h","i","j","k","l","m",
            "n","o","p","q","r","s","t","u","v","w","x","y","z",

            "Ä","Ö","Ü","Â","À","Ç","É","È","Ê","Ë","Î","Ï","Ô","Œ",
            "Ù","Û","Á","Í","Ñ","Ó","Ú","Ì","Ò",
            "ä","ö","ü","ß","é","â","à","ç","è","ê","ë","î","ï","ô","œ",
            "ù","û","á","í","ñ","ó","ú","ì","ò",

            "°","[er]","[re]","e","¿","¡","a",
            "”","“","’","‘","«","»",

            "0","1","2","3","4","5","6","7","8","9",
            "…","#","„","“","‚","‘",

            "#","#","#","#","#","#","#","#","#","#",
            "#","#","#","#","#","#","#","#","#","#",
            "#","#","#","#","#","#","#","#","#","#",
            "#","#","#","#","#","#","#","#","#","#",
            "#","#","#","#","#","#","#","#","#","#",
            "#","#","#","#","#","#","#","#","#","#",
            "#","#",

            "[Mushroom]","⚘","★","●","♥",

            //Monospaced
            "A","B","C","D","E","F","G","H","I","J","K","L","M",
            "N","O","P","Q","R","S","T","U","#","#",

            "?","!","-"," ",

            "⁰","¹","²","³","⁴","⁵","⁶","⁷","⁸","⁹",
            "₀","₁","₂","₃","₄","₅","₆","₇","₈","₉",

            //Monospaced (except e)
            "e","V","W","X","Y","Z",

            "[Promotional]","[Null]"
        };

        public static byte[] spriteSize =
	    {
		    4,6,4,4,4,4,4,4,4,4,4,4,4,4,4,5,
		    4,4,4,4,4,4,4,4,4,4,4,4,6,4,4,4,
		    4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,
		    4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,
		    4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,
		    4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,
		    4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,
		    4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,
		    4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,
		    4,4,4,4,4,4,4,4,4,4,4,5,4,4,4,4,
		    4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,
		    4,4,4,4,4,4,4,4,4,5,5,4,4,4,4,4,
		    4,4,4,4,4,4,5,6,4,4,5,5,4,4,4,4,
		    4,4,4,5,4,4,4,4,4,4,4,6,4,4,4,5,
		    6,6,4,4,4,5,5,4,4,5,5,4,4,4,4,4,
		    4,4,4,4,4,5,5,6,6,5,5,4,4,5,5,5,
		    5,6,6,4,4,6,6,5,5,5,5,5,5,5,5,4,
		    4,4,5,6,6,4,6,4,4,4,6,6,4,6,6,5,
		    6,4,4,5,4,4,5,6,4,4,5,4,4,5,5,5,
		    5,5,5,5,5,5,5,4,4,4,4,5,5,5,5,5,
		    5,5,5,5,5,5,4,4,4,5,4,4,4,4,4,4,
		    4,5,5,5,5,5,5,5,5,5,5,5,6,5,5,5
	    };

        public static string[] levelSet =
        {
            "e","Star","Mushroom","Flower","Heart",
            "A","B","C","D","E","F","G","H","I","J","K","L","M",
            "N","O","P","Q","R","S","T","U","V","W","X","Y","Z",
            "Promotional"
        };

        public static string[] levelIcon =
        {
            "e+", "Star", "Desert", "Warp Zone", "Fortress", "Castle", "Tower", "Pyramid",
            "Toad House (Yellow)", "Ghost House", "Airship", "Tank", "Airship (Cannon)",
            "Airship (Small)", "Coin Ship", "Hand", "Cloud", "Plains", "Palm Tree", "Water",
            "Flower", "Ice", "Piranha Plant", "Volcano", "Skull", "Hammer Bro.", "Boomerang Bro.",
            "Sledge Bro.", "Fire Bro.", "Invalid", "Invalid", "No Icon"
        };

        //Level Card components
        public static header level_header;
        public static eCoin level_eCoin;
        public static List<room>[] roomData;
    }

    public static class Compression
    {
        //TODO
    }

    static class Program
    {
        /// <summary>
        /// Point d'entrée principal de l'application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }

        static void NewLevel()
        {
            //TODO
        }

        static void LoadLevel()
        {
            //TODO
        }
    }
}
