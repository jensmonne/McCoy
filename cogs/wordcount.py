import discord
from discord.ext import commands

class Wordcount(commands.Cog):
    print("ello")

# Add the Cog to the bot
async def setup(bot):
    await bot.add_cog(Wordcount(bot))