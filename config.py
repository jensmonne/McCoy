import math
from discord.ext import commands
from dotenv import load_dotenv
import os

# Load environment variables
load_dotenv()
ADMIN_ID = int(os.getenv('ADMIN_ID'))

LEVEL_UP_EMOJIS = ["😘", "🫃", "💪", "😼", "👹", "💅", "👻"]
LEVEL_SHOW_EMOJIS = ["😹", "😮", "🙏", "👺", "🙄", "🤓", "🙀"]

# function to calculate XP required for next level (Exponantial growth)
def required_level_xp(level):
    return math.floor((5*(level**2)+(50*level)+100)/2)
#   return math(5*(level**2)+(50*level)+100-required_level_xp)
#    return math.floor(100 * (1.5 ** (level - 1)))

#function to check if user is either an admin or the owner
def is_admin_or_owner():
    async def predicate(ctx):
        return ctx.author.guild_permissions.administrator or ctx.author.id == ADMIN_ID
    return commands.check(predicate)