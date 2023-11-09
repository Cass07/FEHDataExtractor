using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FEHDataExtractor
{
    public class SingleStage : CommonRelated
    {
        StringXor id_tag;
        StringXor base_id;
        StringXor[] prerequisites;
        UInt32Xor prereq_count;

        StringXor honor_id;
        StringXor name_id;
        StringXor _unknown1;

        Reward_Payload reward;

        Int32Xor payload_size;
        UInt32Xor origins;

        UInt16Xor stamina;
        UInt16Xor _unknown2;
        UInt16Xor difficulty;
        Int16Xor survives;
        UInt16Xor no_lights_blessings;
        UInt16Xor turns_to_win;
        UInt16Xor turns_to_defend;
        UInt16Xor stars;

        UInt16Xor lv;
        UInt16Xor true_lv;
        UInt16Xor reinforcement;
        UInt16Xor last_enemy_phase;

        Int16Xor max_refresher;
        Int16Xor rd_level;

        SByteXor[] enemy_weps; //8

        public StringXor Id_tag { get => id_tag; set => id_tag = value; }
        public StringXor Base_id { get => base_id; set => base_id = value; }
        public StringXor[] Prerequisites { get => prerequisites; set => prerequisites = value; }
        public UInt32Xor Prereq_count { get => prereq_count; set => prereq_count = value; }

        public StringXor Honor_id { get => honor_id; set => honor_id = value; }
        public StringXor Name_id { get => name_id; set => name_id = value; }
        public StringXor _Unknown1 { get => _unknown1; set => _unknown1 = value; }

        public Reward_Payload Reward { get => reward; set => reward = value; }

        public Int32Xor Payload_size { get => payload_size; set => payload_size = value; }
        public UInt32Xor Origins { get => origins; set => origins = value; }

        public UInt16Xor Stamina { get => stamina; set => stamina = value; }
        public UInt16Xor _Unknown2 { get => _unknown2; set => _unknown2 = value; }
        public UInt16Xor Difficulty { get => difficulty; set => difficulty = value; }
        public Int16Xor Survives { get => survives; set => survives = value; }
        public UInt16Xor No_lights_blessings { get => no_lights_blessings; set => no_lights_blessings = value; }
        public UInt16Xor Turns_to_win { get => turns_to_win; set => turns_to_win = value; }
        public UInt16Xor Turns_to_defend { get => turns_to_defend; set => turns_to_defend = value; }
        public UInt16Xor Stars { get => stars; set => stars = value; }

        public UInt16Xor Lv { get => lv; set => lv = value; }
        public UInt16Xor True_lv { get => true_lv; set => true_lv = value; }
        public UInt16Xor Reinforcement { get => reinforcement; set => reinforcement = value; }
        public UInt16Xor Last_enemy_phase { get => last_enemy_phase; set => last_enemy_phase = value; }

        public Int16Xor Max_refresher { get => max_refresher; set => max_refresher = value; }
        public Int16Xor Rd_level { get => rd_level; set => rd_level = value; }

        public SByteXor[] Enemy_weps { get => enemy_weps; set => enemy_weps = value; }

        public SingleStage()
        {
            Name = "SingleStage";
            Size = 0x78;
            Prereq_count = new UInt32Xor(0x01, 0xFD, 0x2D, 0x09);
            Payload_size = new Int32Xor(0xe2, 0x5E, 0x64, 0x64);
            Origins = new UInt32Xor(0x02, 0x0B, 0x08, 0x67);
            Stamina = new UInt16Xor(0x22, 0xBB);
            _Unknown2 = new UInt16Xor(0x35, 0x23);
            Difficulty = new UInt16Xor(0x74, 0xC0);
            Survives = new Int16Xor(0xCB, 0x4F);
            No_lights_blessings = new UInt16Xor(0x30, 0x30);
            Turns_to_win = new UInt16Xor(0x43, 0xA7);
            Turns_to_defend = new UInt16Xor(0x91, 0x80);
            Stars = new UInt16Xor(0xB6, 0xCB);
            Lv = new UInt16Xor(0xBA, 0x14);
            True_lv = new UInt16Xor(0x53, 0xD9);
            Reinforcement = new UInt16Xor(0x99, 0x63);
            Last_enemy_phase = new UInt16Xor(0x4A, 0x6C);
            Max_refresher = new Int16Xor(0x5B, 0x29);
            Rd_level = new Int16Xor(0x4E, 0x7C);
            Enemy_weps = new SByteXor[8];
            for (int i = 0; i < 8; i++)
            {
                Enemy_weps[i] = new SByteXor(0x97);
            }
        }

        public SingleStage(long a, byte[] data) : this()
        {
            InsertIn(a, data);
        }

        public override void InsertIn(long a, byte[] data)
        {
            Id_tag = new StringXor(ExtractUtils.getLong(a, data) + offset, data, Common);
            Archive.Index++;
            Base_id = new StringXor(ExtractUtils.getLong(a + 8, data) + offset, data, Common);
            Archive.Index++;
            Prereq_count.XorValue(ExtractUtils.getInt(a + 0x18, data));
            Prerequisites = new StringXor[Prereq_count.Value];
            long pos = ExtractUtils.getLong(a + 0x10, data) + offset;
            for (int i = 0; i < Prereq_count.Value; i++)
            {
                Prerequisites[i] = new StringXor(ExtractUtils.getLong(pos + 0x08 * i, data) + offset, data, Common);
                Archive.Index++;
            }
            Honor_id = new StringXor(ExtractUtils.getLong(a + 0x20, data) + offset, data, Common);
            Archive.Index++;
            Name_id = new StringXor(ExtractUtils.getLong(a + 0x28, data) + offset, data, Common);
            Archive.Index++;
            _Unknown1 = new StringXor(ExtractUtils.getLong(a + 0x30, data) + offset, data, Common);
            Archive.Index++;
            //Reward.InsertIn(a + 0x38, data);


            Payload_size.XorValue(ExtractUtils.getInt(a + 0x40, data));
            Reward = new Reward_Payload(Payload_size.Value);
            Archive.Index++;
            Reward.InsertIn(Archive, ExtractUtils.getLong(a + 0x38, data) + offset, data);

            Origins.XorValue(ExtractUtils.getInt(a + 0x44, data));
            Stamina.XorValue(ExtractUtils.getShort(a + 0x48, data));
            _Unknown2.XorValue(ExtractUtils.getShort(a + 0x4A, data));
            Difficulty.XorValue(ExtractUtils.getShort(a + 0x4C, data));
            Survives.XorValue(ExtractUtils.getShort(a + 0x56, data));
            No_lights_blessings.XorValue(ExtractUtils.getShort(a + 0x58, data));
            Turns_to_win.XorValue(ExtractUtils.getShort(a + 0x5A, data));
            Turns_to_defend.XorValue(ExtractUtils.getShort(a + 0x5C, data));
            Stars.XorValue(ExtractUtils.getShort(a + 0x5E, data));
            Lv.XorValue(ExtractUtils.getShort(a + 0x60, data));
            True_lv.XorValue(ExtractUtils.getShort(a + 0x62, data));
            Reinforcement.XorValue(ExtractUtils.getShort(a + 0x64, data));
            Last_enemy_phase.XorValue(ExtractUtils.getShort(a + 0x66, data));
            Max_refresher.XorValue(ExtractUtils.getShort(a + 0x68, data));
            Rd_level.XorValue(ExtractUtils.getShort(a + 0x6A, data));
            for (int i = 0; i < 8; i++)
            {
                Enemy_weps[i].XorValue(data[a + 0x70 + 0x01 * i]);
            }
        }

        public override string ToString()
        {
            String text = "";
            text += "------------------------------------------------------" + Environment.NewLine;
            text += "ID_Tag : " + Id_tag.Value + Environment.NewLine;
            text += "Base_ID : " + Base_id.Value + Environment.NewLine;

            text += "Prerequisites : [";
            for (int i = 0; i < Prereq_count.Value; i++)
            {
                text += Prerequisites[i].Value + ",";
            }

            text += "]" + Environment.NewLine; ;
            text += "Prereq Count : " + Prereq_count.Value + Environment.NewLine;

            text += !Honor_id.Value.Equals("") ? "Honor ID : " + Honor_id.Value + Environment.NewLine : "";
            text += Table.Contains(Honor_id.Value) ? "Honor : " + Table[Honor_id.Value] + Environment.NewLine : "";
            text += TableJP.Contains(Honor_id.Value) ? "Honor JP : " + TableJP[Honor_id.Value] + Environment.NewLine : "";
            text += !Name_id.Value.Equals("") ? "Name ID : " + Name_id.Value + Environment.NewLine : "";
            text += Table.Contains(Name_id.Value) ? "Name : " + Table[Name_id.Value] + Environment.NewLine : "";
            text += TableJP.Contains(Name_id.Value) ? "Name JP : " + TableJP[Name_id.Value] + Environment.NewLine : "";
            text += !_Unknown1.Value.Equals("") ? "Unknown Value 1 : " + _Unknown1.Value + Environment.NewLine : "";

            text += Reward + Environment.NewLine;

            text += "Payload Size : " + Payload_size.Value + Environment.NewLine;
            text += "Origins : " + Origins.Value + Environment.NewLine;
            text += "Stamina : " + Stamina.Value + Environment.NewLine;
            text += "Unknown Value 2 : " + _Unknown2.Value + Environment.NewLine;
            text += "Difficulty : " + Difficulty.Value + Environment.NewLine;
            text += "Survives : " + Survives.Value + Environment.NewLine;
            text += "No Lights Blessing : " + No_lights_blessings.Value + Environment.NewLine;
            text += "Turns to Win : " + Turns_to_win.Value + Environment.NewLine;
            text += "Turns to Defend : " + Turns_to_defend.Value + Environment.NewLine;
            text += "Stars : " + Stars.Value + Environment.NewLine;
            text += "Level : " + Lv.Value + Environment.NewLine;
            text += "True Level : " + True_lv.Value + Environment.NewLine;
            text += "Last Enemy Phase : " + Last_enemy_phase.Value + Environment.NewLine;
            text += "Reinforcement : " + Reinforcement.Value + Environment.NewLine;
            text += "Max Refreshers : " + Max_refresher.Value + Environment.NewLine;
            text += "RD Level : " + Rd_level.Value + Environment.NewLine;
            text += "Enemy Weps : [";
            for (int i = 0; i < 8; i++)
            {
                text += Enemy_weps[i] + ",";
            }
            text += "]" + Environment.NewLine;
            text += "------------------------------------------------------" + Environment.NewLine;
            return text;
        }
        public override string ToString_json()
        {
            return this.ToString();
        }
    }

    public class StageEvent : CommonRelated
    {
        StringXor id_tag;
        StringXor banner_id;
        StringXor rd_tag;
        StringXor rd_bonus;
        StringXor _unknown1;

        Int64Xor start;
        Int64Xor finish;
        Int64Xor avail_sec;
        Int64Xor cycle_sec;

        UInt32Xor sort_id1;
        UInt32Xor sort_id2;

        SingleStage[] scenarios;

        UInt32Xor scenario_count;
        SByteXor rival_domains;

        public StringXor Id_tag { get => id_tag; set => id_tag = value; }
        public StringXor Banner_id { get => banner_id; set => banner_id = value; }
        public StringXor Rd_tag { get => rd_tag; set => rd_tag = value; }
        public StringXor Rd_bonus { get => rd_bonus; set => rd_bonus = value; }
        public StringXor _Unknown1 { get => _unknown1; set => _unknown1 = value; }

        public Int64Xor Start { get => start; set => start = value; }
        public Int64Xor Finish { get => finish; set => finish = value; }
        public Int64Xor Avail_sec { get => avail_sec; set => avail_sec = value; }
        public Int64Xor Cycle_sec { get => cycle_sec; set => cycle_sec = value; }

        public UInt32Xor Sort_id1 { get => sort_id1; set => sort_id1 = value; }
        public UInt32Xor Sort_id2 { get => sort_id2; set => sort_id2 = value; }
        public SingleStage[] Scenarios { get => scenarios; set => scenarios = value; }

        public UInt32Xor Scenario_count { get => scenario_count; set => scenario_count = value; }
        public SByteXor Rival_domains { get => rival_domains; set => rival_domains = value; }

        public StageEvent()
        {
            Name = "StageEvent";
            Size = 0x68;
            ElemXor = new byte[] { 0x17, 0x91, 0x73, 0xBB, 0xA7, 0xFF, 0x4A, 0xC8 };
            Start = new Int64Xor(0x60, 0xf6, 0x37, 0xc5, 0x36, 0xa2, 0x0d, 0xdc);
            Finish = new Int64Xor(0xE9, 0x56, 0xBD, 0xFA, 0x2A, 0x69, 0xAD, 0xC8);
            Avail_sec = new Int64Xor(0xE0, 0xA6, 0x08, 0x41, 0x40, 0xF0, 0x11, 0x73);
            Cycle_sec = new Int64Xor(0x21, 0x57, 0x1B, 0xD5, 0xC9, 0x7E, 0x22, 0x6B);

            Sort_id1 = new UInt32Xor(0xB7, 0xF0, 0xBD, 0xA2);
            Sort_id2 = new UInt32Xor(0xB7, 0xF6, 0x6E, 0xFE);

            Scenario_count = new UInt32Xor(0x66, 0xB2, 0xBF, 0xFD);

            Rival_domains = new SByteXor(0x34);

        }

        public StageEvent(long a, byte[] data) : this()
        {
            InsertIn(a, data);
        }

        public override void InsertIn(long a, byte[] data)
        {
            Id_tag = new StringXor(ExtractUtils.getLong(a, data) + offset, data, Common);
            Archive.Index++;
            Banner_id = new StringXor(ExtractUtils.getLong(a + 8, data) + offset, data, Common);
            Archive.Index++;
            Rd_tag = new StringXor(ExtractUtils.getLong(a + 0x10, data) + offset, data, Common);
            Archive.Index++;
            Rd_bonus = new StringXor(ExtractUtils.getLong(a + 0x18, data) + offset, data, Common);
            Archive.Index++;
            _Unknown1 = new StringXor(ExtractUtils.getLong(a + 0x20, data) + offset, data, Common);
            Archive.Index++;
            Start.XorValue(ExtractUtils.getLong(a + 0x28, data));
            Finish.XorValue(ExtractUtils.getLong(a + 0x30, data));
            Avail_sec.XorValue(ExtractUtils.getLong(a + 0x38, data));
            Cycle_sec.XorValue(ExtractUtils.getLong(a + 0x40, data));

            Sort_id1.XorValue(ExtractUtils.getInt(a + 0x50, data));
            Sort_id2.XorValue(ExtractUtils.getInt(a + 0x54, data));
            Scenario_count.XorValue(ExtractUtils.getInt(a + 0x60, data));
            Scenarios = new SingleStage[Scenario_count.Value];

            for (int i = 0; i < Scenario_count.Value; i++)
            {
                Scenarios[i] = new SingleStage();
                Scenarios[i].InsertIn(Archive, ExtractUtils.getLong(a + 0x58, data) + offset + i * Scenarios[i].Size, data);
            }

            Rival_domains.XorValue(data[a + 0x65]);
        }

        public override string ToString()
        {
            String text = "";
            text += "ID_Tag : " + Id_tag.Value + Environment.NewLine;
            text += "Banner_id : " + Banner_id.Value + Environment.NewLine;
            text += Table.Contains("MID_STAGE_" + Banner_id.Value) ? "Title : " + Table["MID_STAGE_" + Banner_id.Value] + " - " + Table["MID_STAGE_HONOR_" + Banner_id.Value] + Environment.NewLine : "";
            text += TableJP.Contains("MID_STAGE_" + Banner_id.Value) ? "Title JP : " + TableJP["MID_STAGE_" + Banner_id.Value] + " - " + TableJP["MID_STAGE_HONOR_" + Banner_id.Value] + Environment.NewLine : "";

            text += "RD_Tag : " + Rd_tag.Value + Environment.NewLine;
            text += "RD_Bonus : " + Rd_bonus.Value + Environment.NewLine;
            text += "Start : ";
            text += Start.Value < 0 ? "Not available" + Environment.NewLine : DateTimeOffset.FromUnixTimeSeconds(Start.Value).DateTime.ToLocalTime() + Environment.NewLine;
            text += "Finish : ";
            text += Finish.Value < 0 ? "Not available" + Environment.NewLine : DateTimeOffset.FromUnixTimeSeconds(Finish.Value).DateTime.ToLocalTime() + Environment.NewLine;

            text += "Avail Sec : " + Avail_sec.Value + Environment.NewLine;
            text += "Cycle Sec : " + Cycle_sec.Value + Environment.NewLine;

            text += "Sort Id 1 : " + Sort_id1.Value + Environment.NewLine;
            text += "Sort Id 2 : " + Sort_id2.Value + Environment.NewLine;

            text += "Scenario Count : " + Scenario_count.Value + Environment.NewLine;
            for (int i = 0; i < Scenario_count.Value; i++)
            {
                text += Scenarios[i];

            }
            text += "Rival Domains : " + Rival_domains.Value + Environment.NewLine;
            text += "-----------------------------------------------------------------------------------------------" + Environment.NewLine;

            return text;
        }

        public override string ToString_json()
        {
            return this.ToString();
        }
    }

    public class SingleUnit : CommonRelated
    {
        StringXor id_tag;
        StringXor[] skills; //8
        StringXor accessory;

        SinglePosition position;

        ByteXor rarity;
        ByteXor lv;

        SByteXor cooldown_count;
        SByteXor max_cooldown_count;

        Stats stats;
        SByteXor start_turn;

        SByteXor movement_group;
        SByteXor movement_delay;

        SByteXor break_terrain;
        SByteXor tether;
        ByteXor true_lv;
        SByteXor is_enemy;

        StringXor spawn_check;
        SByteXor spawn_count;
        SByteXor spawn_turns;
        SByteXor spawn_target_remain;
        SByteXor spawn_target_kills;




        public StringXor Id_tag { get => id_tag; set => id_tag = value; }

        public StringXor[] Skills { get => skills; set => skills = value; }

        public StringXor Accessory { get => accessory; set => accessory = value; }

        public SinglePosition Position { get => position; set => position = value; }

        public ByteXor Rarity { get => rarity; set => rarity = value; }

        public ByteXor Lv { get => lv; set => lv = value; }

        public SByteXor Cooldown_count { get => cooldown_count; set => cooldown_count = value; }

        public SByteXor Max_cooldown_count { get => max_cooldown_count; set => max_cooldown_count = value; }

        public Stats Stats_ { get => stats; set => stats = value; }

        public SByteXor Start_turn { get => start_turn; set => start_turn = value; }

        public SByteXor Movement_group { get => movement_group; set => movement_group = value; }

        public SByteXor Movement_delay { get => movement_delay; set => movement_delay = value; }

        public SByteXor Break_terrain { get => break_terrain; set => break_terrain = value; }

        public SByteXor Tether { get => tether; set => tether = value; }

        public ByteXor True_lv { get => true_lv; set => true_lv = value; }

        public SByteXor Is_enemy { get => is_enemy; set => is_enemy = value; }

        public StringXor Spawn_check { get => spawn_check; set => spawn_check = value; }

        public SByteXor Spawn_count { get => spawn_count; set => spawn_count = value; }
        public SByteXor Spawn_turns { get => spawn_turns; set => spawn_turns = value; }
        public SByteXor Spawn_target_remain { get => spawn_target_remain; set => spawn_target_remain = value; }
        public SByteXor Spawn_target_kills { get => spawn_target_kills; set => spawn_target_kills = value; }

        public SingleUnit()
        {
            Name = "SingleUnit";
            Size = 0x80;
            Position = new SinglePosition();

            Rarity = new ByteXor(0x61);
            Lv = new ByteXor(0x2A);

            Cooldown_count = new SByteXor(0x1E);
            Max_cooldown_count = new SByteXor(0x9B);

            Start_turn = new SByteXor(0xCF);
            Movement_group = new SByteXor(0xF4);
            Movement_delay = new SByteXor(0x95);

            Break_terrain = new SByteXor(0x71);

            Tether = new SByteXor(0xB8);
            True_lv = new ByteXor(0x85);
            Is_enemy = new SByteXor(0xD0);

            Spawn_count = new SByteXor(0x0A);
            Spawn_turns = new SByteXor(0x0A);
            Spawn_target_remain = new SByteXor(0x2D);
            Spawn_target_kills = new SByteXor(0x5B);
        }

        public SingleUnit(long a, byte[] data) : this()
        {
            InsertIn(a, data);
        }

        public override void InsertIn(long a, byte[] data)
        {
            Id_tag = new StringXor(ExtractUtils.getLong(a + offset, data) + offset, data, Common);
            Archive.Index++;
            Skills = new StringXor[8];
            for (int i = 0; i < 8; i++)
            {
                Skills[i] = new StringXor(ExtractUtils.getLong(a + offset + 0x08 + 0x08 * i, data) + offset, data, Common);
                Archive.Index++;
            }
            Accessory = new StringXor(ExtractUtils.getLong(a + offset + 0x48, data) + offset, data, Common);
            Archive.Index++;
            Position.InsertIn(Archive, a + 0x50, data);

            Rarity.XorValue(data[a + offset + 0x54]);
            Lv.XorValue(data[a + offset + 0x55]);

            Cooldown_count.XorValue(data[a + offset + 0x56]);
            Max_cooldown_count.XorValue(data[a + offset + 0x57]);

            Stats_ = new Stats();
            Stats_.InsertIn(a + 0x58 + offset, data);

            Start_turn.XorValue(data[a + offset + 0x68]);

            Movement_group.XorValue(data[a + offset + 0x69]);
            Movement_delay.XorValue(data[a + offset + 0x6A]);

            Break_terrain.XorValue(data[a + offset + 0x6B]);

            Tether.XorValue(data[a + offset + 0x6C]);
            True_lv.XorValue(data[a + offset + 0x6D]);
            Is_enemy.XorValue(data[a + offset + 0x6E]);

            Spawn_check = new StringXor(ExtractUtils.getLong(a + offset + 0x70, data) + offset, data, Common);

            Spawn_count.XorValue(data[a + offset + 0x78]);
            Spawn_turns.XorValue(data[a + offset + 0x79]);
            Spawn_target_remain.XorValue(data[a + offset + 0x7A]);
            Spawn_target_kills.XorValue(data[a + offset + 0x7B]);


        }

        public override string ToString()
        {
            String text = "";
            text += "------------------------------------------------------" + Environment.NewLine;
            text += "ID_Tag : " + Id_tag.Value + Environment.NewLine;

            text += "Skills : [";
            for (int i = 0; i < 8; i++)
            {
                text += Skills[i].Value + ",";
            }
            text = text.Substring(0, text.Length - 1);

            text += "]" + Environment.NewLine; ;

            text += "Accesory_ID : " + (Accessory.Value != "" ? Accessory.Value : "null") + Environment.NewLine;

            text += Position.ToString();

            text += "Rarity : " + Rarity.Value + Environment.NewLine; ;

            text += "Level : " + Lv.Value + Environment.NewLine;

            text += "Cooldown_count : " + Cooldown_count.Value + Environment.NewLine;
            text += "Max_cooldown_count : " + Max_cooldown_count.Value + Environment.NewLine;

            text += Stats_.ToString();

            text += "Start_turn : " + Start_turn.Value + Environment.NewLine;

            text += "Movement_group : " + Movement_group.Value + Environment.NewLine;
            text += "Movement_delay : " + Movement_delay.Value + Environment.NewLine;

            text += "Break_terrain : " + (Break_terrain.Value == 1) + Environment.NewLine;
            text += "Tether : " + (Tether.Value == 1) + Environment.NewLine;
            text += "True_lv : " + True_lv.Value + Environment.NewLine;
            text += "Is_enemy : " + (Is_enemy.Value == 1) + Environment.NewLine;

            text += "Spawn_check : " + (Spawn_check.Value != "" ? Spawn_check.Value : "null") + Environment.NewLine;

            text += "Spawn_count : " + Spawn_count.Value + Environment.NewLine;
            text += "Spawn_turns : " + Spawn_turns.Value + Environment.NewLine;
            text += "Spawn_target_remain : " + Spawn_target_remain.Value + Environment.NewLine;
            text += "Spawn_target_kills : " + Spawn_target_kills.Value + Environment.NewLine;

            text += "------------------------------------------------------" + Environment.NewLine;
            return text;
        }
        public override string ToString_json()
        {
            return this.ToString();
        }
    }

    public class SinglePosition : CommonRelated
    {
        UInt16Xor pos_x;
        UInt16Xor pos_y;

        public UInt16Xor Pos_x { get => pos_x; set => pos_x = value; }

        public UInt16Xor Pos_y { get => pos_y; set => pos_y = value; }


        public SinglePosition()
        {
            Name = "SinglePosition";
            Size = 0x08;
            Pos_x = new UInt16Xor(0x32, 0xB3);
            Pos_y = new UInt16Xor(0xB2, 0x28);

        }

        public SinglePosition(long a, byte[] data) : this()
        {
            InsertIn(a, data);
        }

        public override void InsertIn(long a, byte[] data)
        {
            Pos_x.XorValue(ExtractUtils.getShort(a + offset, data));
            Pos_y.XorValue(ExtractUtils.getShort(a + offset + 0x02, data));
        }

        public override string ToString()
        {
            return "Pos : (" + Pos_x.Value + "," + Pos_y.Value + ")" + Environment.NewLine; ;
        }
        public override string ToString_json()
        {
            return this.ToString();
        }
    }

    public class SingleMap : CommonRelated
    {
        Int32Xor highest_score;
        StringXor field_id;
        Int32Xor width;
        Int32Xor height;
        ByteXor base_terrain;
        ByteXor[,] terrain; //width height

        SinglePosition[] positions;

        SingleUnit[] units;
        Int32Xor player_count;
        Int32Xor unit_count;
        ByteXor turns_to_win;
        ByteXor last_enemy_phase;
        ByteXor turns_to_defend;


        Int32Xor Highest_score { get => highest_score; set => highest_score = value; }

        StringXor Field_id { get => field_id; set => field_id = value; }

        Int32Xor Width { get => width; set => width = value; }
        Int32Xor Height { get => height; set => height = value; }

        ByteXor Base_terrain { get => base_terrain; set => base_terrain = value; }
        ByteXor[,] Terrain { get => terrain; set => terrain = value; }

        SinglePosition[] Positions { get => positions; set => positions = value; }
        SingleUnit[] Units { get => units; set => units = value; }

        Int32Xor Player_count { get => player_count; set => player_count = value; }

        Int32Xor Unit_count { get => unit_count; set => unit_count = value; }

        ByteXor Turns_to_win { get => turns_to_win; set => turns_to_win = value; }
        ByteXor Last_enemy_phase { get => last_enemy_phase; set => last_enemy_phase = value; }
        ByteXor Turns_to_defend { get => turns_to_defend; set => turns_to_defend = value; }

        public SingleMap()
        {
            Name = "SingleMap";
            Size = 0x20;

            Highest_score = new Int32Xor(0xB1, 0x50, 0xE2, 0xA9);
            Width = new Int32Xor(0x5F, 0xD7, 0x7C, 0x6B);
            Height = new Int32Xor(0xD5, 0x12, 0xAA, 0x2B);
            Base_terrain = new ByteXor(0x41);

            Player_count = new Int32Xor(0x9A, 0xC7, 0x63, 0x9D);

            Unit_count = new Int32Xor(0xEE, 0x10, 0x67, 0xAC);

            Turns_to_win = new ByteXor(0xFD);
            Last_enemy_phase = new ByteXor(0xC7);
            Turns_to_defend = new ByteXor(0xEC);
        }

        public SingleMap(long a, byte[] data) : this()
        {
            InsertIn(a, data);
        }

        public override void InsertIn(long a, byte[] data)
        {
            a = offset;
            Highest_score.XorValue(ExtractUtils.getInt(a + 0x04, data));

            Field_id = new StringXor(ExtractUtils.getLong(ExtractUtils.getLong(a + 0x08, data) + offset, data) + offset, data, Common);
            Archive.Index++;

            Width.XorValue(ExtractUtils.getInt(ExtractUtils.getLong(a + 0x08, data) + offset + 0x08, data));

            Height.XorValue(ExtractUtils.getInt(ExtractUtils.getLong(a + 0x08, data) + offset + 0x0C, data));

            Terrain = new ByteXor[Width.Value, Height.Value];


            for (int x = 0; x < Width.Value; x++)
            {
                for (int y = 0; y < Height.Value; y++)
                {
                    Terrain[x, y] = new ByteXor(0xA1);
                    Terrain[x, y].XorValue(data[ExtractUtils.getLong(a + 0x08, data) + offset + 0x18 + 0x01 * x
                        + 0x01 * Width.Value * y]);
                }
            }


            Player_count.XorValue(ExtractUtils.getInt(a + 0x20, data));
            Unit_count.XorValue(ExtractUtils.getInt(a + 0x24, data));
            Units = new SingleUnit[Unit_count.Value];

            for (int i = 0; i < Unit_count.Value; i++)
            {
                Units[i] = new SingleUnit();
                Units[i].InsertIn(Archive, ExtractUtils.getLong(a + 0x18, data) + i * Units[i].Size, data);
            }

            Positions = new SinglePosition[Player_count.Value];
            for (int i = 0; i < Player_count.Value; i++)
            {
                Positions[i] = new SinglePosition();
                Positions[i].InsertIn(Archive, ExtractUtils.getLong(a + 0x10, data) + i * Positions[i].Size, data);
            }

        }

        public override string ToString()
        {
            String text = "";
            text += "------------------------------------------------------" + Environment.NewLine;
            text += "Highest_score : " + Highest_score.Value + Environment.NewLine;
            text += "ID : " + Field_id.Value + Environment.NewLine;

            text += "Width : " + Width.Value + Environment.NewLine;
            text += "Height : " + Height.Value + Environment.NewLine;

            for (int y = Height.Value - 1; y >= 0; y--)
            {
                for (int x = 0; x < Width.Value; x++)
                {
                    text += Terrain[x, y].Value + " ";
                }
                text += Environment.NewLine;
            }
            text += Environment.NewLine;

            for (int i = 0; i < Player_count.Value; i++)
            {
                text += Positions[i].ToString();
            }

            for (int i = 0; i < Unit_count.Value; i++)
            {
                text += Units[i].ToString();
            }

            text += "Player_count : " + Player_count.Value + Environment.NewLine;
            text += "Unit_count : " + Unit_count.Value + Environment.NewLine;

            text += "Turns_to_win : " + Turns_to_win.Value + Environment.NewLine;
            text += "Last_enemy_phase : " + Last_enemy_phase.Value + Environment.NewLine;
            text += "Turns_to_defend : " + Turns_to_defend.Value + Environment.NewLine;


            text += "------------------------------------------------------" + Environment.NewLine;
            return text;
        }
        public override string ToString_json()
        {
            return this.ToString();
        }
    }
}
