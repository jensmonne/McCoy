import discord
import random
import json
from discord.ext import commands
from config import required_level_xp, EMOJIS

class Leveling(commands.Cog):
    def __init__(self, bot):
        self.bot = bot
        self.xp_data = self.load_xp()

    # Load XP data from file
    def load_xp():
        try:
            with open("xp_data.json", "r") as f:
                return json.load(f)
        except FileNotFoundError:
            return {}

    # Save XP data to file
    def save_xp(data):
        with open("xp_data.json", "w") as f:
            json.dump(data, f, indent=4)

    @commands.Cog.listener()
    async def on_message(self, message):
        if message.author.bot:
            return # Ignore messages from the bot itself

        user_id = str(message.author.id)
        guild_id = str(message.guild.id)

        # Ensure guild data exists
        if guild_id not in self.xp_data:
            self.xp_data[guild_id] = {}

        # Ensure user data exists
        if user_id not in self.xp_data[guild_id]:
            self.xp_data[guild_id][user_id] = {"xp": 0, "level": 0}

        # Give random XP
        level = random.randint(5, 10)
        self.xp_data[guild_id][user_id]["xp"] += level
        # GARRY I NEED YOUR FORMULA 🧪🔬

        level = self.xp_data[guild_id][user_id]["level"]
        xp_needed = required_level_xp(level)

        if self.xp_data[guild_id][user_id]["xp"] >= xp_needed:
            self.xp_data[guild_id][user_id]["level"] += 1
            self.xp_data[guild_id][user_id]["xp"] = 0
            await message.channel.send(f"Congratulations {message.author.mention}, you have leveled up to level {self.xp_data[guild_id][user_id]['level']}!")

        #Save XP data
        self.save_xp(self.xp_data)

    @commands.command(name="level")
    async def level(self, ctx):
        user_id = str(ctx.author.id)
        guild_id = str(ctx.guild.id)

        if guild_id in self.xp_data and user_id in self.xp_data[guild_id]:
            level = self.xp_data[guild_id][user_id]["level"]
            xp = self.xp_data[guild_id][user_id]["xp"]
            xp_needed_next = required_level_xp(level)

            # Pick a random emoji
            random_emoji = random.choice(EMOJIS)

            await ctx.send(f"{random_emoji} {ctx.author.mention}, you are level **Level {level}** with **{xp}/{xp_needed_next} XP**.")
        else:
            await ctx.send(f"🤔 {ctx.author.mention}, you haven't earned any XP yet!")

# Add the Cog to the bot
def setup(bot):
    bot.add_cog(Leveling(bot))