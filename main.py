import discord
import os
import random
import json
import math
from dotenv import load_dotenv

# Load environment variables
load_dotenv()
DISCORD_TOKEN = os.getenv('DISCORD_TOKEN')

# Enable necessary intents
intents = discord.Intents.default()
intents.message_content = True
intents.guilds = True
intents.members = True

# Create discord client
client = discord.Client(intents=intents)

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

# function to calculate XP required for next level (Exponantial growth)
def required_level_xp(level):
    return math.floor(100 * (1.5 ** (level - 1)))

# Set finction to a variable
xp_data = load_xp()

@client.event
async def on_ready():
    print(f'We have logged in as {client.user}')

@client.event
async def on_message(message):
    if message.author == client.user:
        return # Ignore messages from the bot itself

    user_id = str(message.author.id)
    guild_id = str(message.guild.id)

    # Ensure guild data exists
    if guild_id not in xp_data:
        xp_data[guild_id] = {}

    # Ensure user data exists
    if user_id not in xp_data[guild_id]:
        xp_data[guild_id][user_id] = {"xp": 0, "level": 0}

    # Give random XP
    level = random.randint(5, 10)
    xp_data[guild_id][user_id]["xp"] += level
    # GARRY I NEED YOUR FORMULA 🧪🔬

    level = xp_data[guild_id][user_id]["level"]
    xp_needed = required_level_xp(level)

    if xp_data[guild_id][user_id]["xp"] >= xp_needed:
        xp_data[guild_id][user_id]["level"] += 1
        xp_data[guild_id][user_id]["xp"] = 0
        await message.channel.send(f"Congratulations {message.author.mention}, you have leveled up to level {xp_data[guild_id][user_id]['level']}!")

    save_xp(xp_data)

    # Command to check level
    if message.content == '#level' or message.content == '#l':
        level = xp_data[guild_id][user_id]["level"]
        xp = xp_data[guild_id][user_id]["xp"]
        xp_needed_next = required_level_xp(level)
        await message.channel.send(f"🤯 {message.author.mention}, you are **Level {level}** with **{xp}/{xp_needed_next} XP**.")

client.run(DISCORD_TOKEN)