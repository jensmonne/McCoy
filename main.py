import discord
import os
from dotenv import load_dotenv
from discord.ext import commands

# Load environment variables
load_dotenv()
DISCORD_TOKEN = os.getenv('DISCORD_TOKEN')

# Enable necessary intents
intents = discord.Intents.default()
intents.message_content = True
intents.guilds = True
intents.members = True

# Initialize bot
bot = commands.Bot(command_prefix="$", intents=intents)

# Load cogs dynamically
COGS = [
    "cogs.events",
    "cogs.leveling",
    "cogs.impregnate",
    "cogs.vcjumpscare",
    "cogs.birthday",
    "cogs.vckick",
    "cogs.soundboard",
]

# Send a message to the console when the bot is ready
@bot.event
async def on_ready():
    print(f"Logged in as {bot.user}")
    await load_cogs()

async def load_cogs():
    for cog in COGS:
        try:
            await bot.load_extension(cog)
            print(f"Loaded {cog}")
        except Exception as e:
            print(f"Failed to load {cog}: {e}")

# run the bot
bot.run(DISCORD_TOKEN)