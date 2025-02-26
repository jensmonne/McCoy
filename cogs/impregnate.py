import random
from discord.ext import commands
from dotenv import load_dotenv
import os

# Load environment variables
load_dotenv()
PREGNANT_USER_ID = int(os.getenv('PREGNANT_USER_ID'))

class Impregnate(commands.Cog):
    def __init__(self, bot):
        self.bot = bot

    async def get_pregnant_emoji(self):
        # Randomize impregnation
        chance = random.randint(1, 180)

        if chance == 42:
            return "🤰" 
        else:
            return "🫃"

    @commands.Cog.listener()
    async def on_message(self, message):
        # Ignore messages from the bot itself
        if message.author.bot:
            return
                
        # Ignore messages from non-pregnant users
        if message.author.id != PREGNANT_USER_ID:
            return
        
        if (random.randint(1, 12) == 3):
            pregnant_emoji = await self.get_pregnant_emoji()
            # Impregnate the user
            await message.add_reaction(pregnant_emoji)
        else:
            return
        
# Add the Cog to the bot
async def setup(bot):
    await bot.add_cog(Impregnate(bot))