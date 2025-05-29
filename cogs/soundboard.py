import discord
import os
from discord.ext import commands

# Windows
# AUDIO_FOLDER = "assets/soundboard"

# Linux
AUDIO_FOLDER = "/home/jeans/bot/McCoy/assets/soundboard"

class Soundboard(commands.Cog):
    def __init__(self, bot):
        self.bot = bot

    def get_audio_file(self, file_name):
        file_path = os.path.join(AUDIO_FOLDER, file_name)
        if os.path.isfile(file_path + '.mp3'):
            return file_path + '.mp3'
        elif os.path.isfile(file_path + '.wav'):
            return file_path + '.wav'
        else:
            return None

    @commands.command(name="sb", case_insensitive=True)
    async def play_sound(self, ctx, *, file_name: str):
        if not ctx.author.voice or not ctx.author.voice.channel:
            await ctx.send("You must be in a voice channel to use this command.")
            return
        
        vc = ctx.author.voice.channel
        audio_file = self.get_audio_file(file_name)
        if not audio_file:
            await ctx.send(f"No audio file found with the name '{file_name}'.")
            return
        
        try:
            voice_client = await vc.connect()
            print(f"Joined {vc.name} in {vc.guild.name} to play {audio_file}")

            source = discord.FFmpegPCMAudio(audio_file)

            def after_playing(error):
                if error:
                    print(f"Error playing audio: {error}")
                self.bot.loop.create_task(self.disconnect_vc(voice_client))
            
            voice_client.play(source, after=lambda error: after_playing(error))

        except discord.ClientException:
            await ctx.send(f"Already in a voice channel in {vc.guild.name}")

    async def disconnect_vc(self, voice_client):
        if voice_client.is_connected():
            await voice_client.disconnect()
            print("Left voice channel after playing audio.")

    @commands.command(name="path", case_insensitive=True)
    async def list_files(self, ctx):
        try:
            files = os.listdir(AUDIO_FOLDER)
            if not files:
                await ctx.send("No files found in the audio folder.")
            else:
                file_list = "\n".join(files)
                await ctx.send(f"Files in the audio folder:\n{file_list}")
        except Exception as e:
            await ctx.send(f"Error accessing the audio folder: {e}")

# Add the Cog to the bot
async def setup(bot):
    await bot.add_cog(Soundboard(bot))
