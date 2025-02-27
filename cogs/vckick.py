from discord.ext import commands
from config import is_admin_or_owner
from dotenv import load_dotenv
import os

# Load environment variables
load_dotenv()
CHLOE_ID = int(os.getenv('CHLOE_ID'))
BRANDON_ID = int(os.getenv('BRANDON_ID'))

class kick(commands.Cog):
    def __init__(self, bot):
        self.bot = bot

    @commands.command(case_insensitive=True)
    @is_admin_or_owner()
    async def kick(self, ctx, user_id: int, *, reason=None):
        user = ctx.guild.get_member(user_id)
        if user is not None and user.voice is not None and user.voice.channel is not None:
            await user.move_to(None)
            print(f"{user} was kicked from the voice channel.")
        else:
            print(f"{user} is not in a voice channel")

    @commands.command(case_insensitive=True)
    @is_admin_or_owner()
    async def chloe(self, ctx):
        user = ctx.guild.get_member(CHLOE_ID)
        if user is not None and user.voice is not None and user.voice.channel is not None:
            await user.move_to(None)
            print("Chloe was a slut!")
        else:
            print("Chloe was not a slut!")

    @commands.command(case_insensitive=True)
    @is_admin_or_owner()
    async def brandon(self, ctx):
        user = ctx.guild.get_member(BRANDON_ID)
        if user is not None and user.voice is not None and user.voice.channel is not None:
            await user.edit(mute=True)
            print(f"Brandon was a pedophile.")
        else:
            print(f"Brandon was not a pedophile.")

# Add the Cog to the bot
async def setup(bot):
    await bot.add_cog(kick(bot))