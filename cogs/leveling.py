import discord
import random
import json
import time
from discord.ext import commands
from config import required_level_xp, is_admin_or_owner, LEVEL_UP_EMOJIS, LEVEL_SHOW_EMOJIS

class Leveling(commands.Cog):
    def __init__(self, bot):
        self.bot = bot
        self.xp_data = self.load_xp()
        self.config = self.load_config()
        self.last_message_time = {}

    ### --- JSON DATA HANDELING --- ###

    # Load XP data from file
    def load_xp(self):
        try:
            with open("xp_data.json", "r") as f:
                return json.load(f)
        except (FileNotFoundError, json.JSONDecodeError):
            return {}

    # Save XP data to file
    def save_xp(self, data):
        with open("xp_data.json", "w") as f:
            json.dump(data, f, indent=4)

    # Load config data from file
    def load_config(self):
        try:
            with open("config.json", "r") as f:
                return json.load(f)
        except (FileNotFoundError, json.JSONDecodeError):
            return {}
        
    # Save config data to file
    def save_config(self, data):
        with open("config.json", "w") as f:
            json.dump(data, f, indent=4)
    
    ### --- LEVELING SYSTEM --- ###

    @commands.Cog.listener()
    async def on_message(self, message):
        # Ignore messages from the bot itself
        if message.author.bot:
            return

        user_id = str(message.author.id)
        guild_id = str(message.guild.id)

        # Check if the message is sent within 10 seconds of the last message
        current_time = time.time()
        if user_id in self.last_message_time:
            last_time = self.last_message_time[user_id]
            if current_time - last_time < 10:
                return 

        # Update the last message time
        self.last_message_time[user_id] = current_time

        # Ensure guild data exists
        if guild_id not in self.xp_data:
            self.xp_data[guild_id] = {}

        # Ensure user data exists
        if user_id not in self.xp_data[guild_id]:
            self.xp_data[guild_id][user_id] = {"xp": 0, "level": 0}

        # Give random XP between 4 and 8
        xp_gain = random.randint(4, 8)
        self.xp_data[guild_id][user_id]["xp"] += xp_gain
        # GARRY I NEED YOUR FORMULA 🧪🔬

        level = self.xp_data[guild_id][user_id]["level"]
        xp_needed = required_level_xp(level)

        # Pick a random emoji
        random_emoji = random.choice(LEVEL_UP_EMOJIS)

        if self.xp_data[guild_id][user_id]["xp"] >= xp_needed:
            old_level = self.xp_data[guild_id][user_id]["level"]

            self.xp_data[guild_id][user_id]["level"] += 1
            self.xp_data[guild_id][user_id]["xp"] = 0

            # If the level is different from the previous one, send a level-up message
            if self.xp_data[guild_id][user_id]["level"] > old_level:
                # Get the custom level-up channel
                channel_id = self.config.get(guild_id, {}).get("levelup_channel")
                if channel_id:
                    levelup_channel = self.bot.get_channel(int(channel_id))
                else:
                    levelup_channel = message.channel # Default to the current channel

            # Send level-up message
            await levelup_channel.send(f"{random_emoji} Congratulations {message.author.mention}, you have leveled up to level {self.xp_data[guild_id][user_id]['level']}!")

        #Save XP data
        self.save_xp(self.xp_data)

    ### --- LEVELING COMMANDS --- ###

    @commands.command(name="level", aliases=["lvl", "l"], case_insensitive=True)
    async def level(self, ctx):
        user_id = str(ctx.author.id)
        guild_id = str(ctx.guild.id)

        random_emoji = random.choice(LEVEL_SHOW_EMOJIS)

        if guild_id in self.xp_data and user_id in self.xp_data[guild_id]:
            level = self.xp_data[guild_id][user_id]["level"]
            xp = self.xp_data[guild_id][user_id]["xp"]
            xp_needed_next = required_level_xp(level)

            await ctx.send(f"{random_emoji} {ctx.author.mention}, you are level **Level {level}** with **{xp}/{xp_needed_next} XP**.")
        else:
            await ctx.send(f"🤔 {ctx.author.mention}, you haven't earned any XP yet!")

    @commands.command(name="setlevelupchannel", aliases=["setlevelchannel", "slc", "setlc"], case_insensitive=True)
    @is_admin_or_owner()
    async def set_levelup_channel(self, ctx, channel: discord.TextChannel):
        guild_id = str(ctx.guild.id)

        # Ensure config exists for the guild
        if guild_id not in self.config:
            self.config[guild_id] = {}

        # get the current level-up channel ID
        current_channel_id = self.config[guild_id].get("levelup_channel")

        # Convert to string for comparison
        new_channel_id = str(channel.id)

        # Check if the new channel is the same as the existing one
        if current_channel_id == new_channel_id:
            await ctx.send(f"Level up messages are already set to {channel.mention}.")
            return

        # update and save the new level-up channel
        self.config[guild_id]["levelup_channel"] = str(new_channel_id)
        self.save_config(self.config)

        await ctx.send(f"Level up messages will now be sent in {channel.mention}")

# Add the Cog to the bot
async def setup(bot):
    await bot.add_cog(Leveling(bot))