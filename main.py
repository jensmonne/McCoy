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
    "cogs.leveling"
]

# Send a message to the console when the bot is ready
@bot.event
async def on_ready():
    print(f"Logged in as {bot.user}")

# load all cogs
for cog in COGS:
    bot.load_extension(cog)

# run the bot
bot.run(DISCORD_TOKEN)