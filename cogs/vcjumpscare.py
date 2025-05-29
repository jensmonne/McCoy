import discord
import random
import os
from discord.ext import commands, tasks

# Windows
# AUDIO_FOLDER = "assets/audio"

# Linux
AUDIO_FOLDER = "/home/jeans/bot/McCoy/assets/audio"

class VCJumpscare(commands.Cog):
    def __init__(self, bot):
        self.bot = bot
        self.random_vc_join.start()

    @tasks.loop(minutes=1)
    async def random_vc_join(self):
        chance = random.randint(1, 180)
        if chance != 1:
            return
        
        active_vcs = []
        for guild in self.bot.guilds:
            for vc in guild.voice_channels:
                if vc.members:
                    active_vcs.append(vc)

        if not active_vcs:
            return
        
        vc = random.choice(active_vcs)
        audio_file = self.get_random_audio_file()
        if not audio_file:
            print("No audio files found")
            return
        
        try:
            voice_client = await vc.connect()
            print(f"Joined {vc.name} in {vc.guild.name} to play {audio_file}")
            source = discord.FFmpegPCMAudio(audio_file)

            def after_playing(error):
                if error:
                    print(f"Error playing audio: {error}")
                self.bot.loop.create_task(self.disconnect_vc(voice_client))
            
            voice_client.play(source, after=after_playing)


        except discord.ClientException:
            print(f"Already in a voice channel in {vc.guild.name}")

    async def disconnect_vc(self, voice_client):
        if voice_client.is_connected():
            await voice_client.disconnect()
            print("Left voice channel after playing audio.")

    def get_random_audio_file(self):
        files = [f for f in os.listdir(AUDIO_FOLDER) if f.endswith(('.mp3', '.wav'))]
        if not files:
            print("No audio files found in the folder.")
            return None
        print(f"Selected audio file: {files[0]}")
        return os.path.join(AUDIO_FOLDER, random.choice(files))


    @commands.command(name="jumpscare", aliases=["jsjoin", "jsj"], case_insensitive=True)
    async def joinvc(self, ctx):
        if not ctx.author.voice or not ctx.author.voice.channel:
            await ctx.send("You must be in a voice channel to use this command.")
            return
        
        vc = ctx.author.voice.channel
        audio_file = self.get_random_audio_file()
        if not audio_file:
            await ctx.send("No audio files found.")
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

    def cog_unload(self):
        self.random_vc_join.cancel()

# Add the Cog to the bot
async def setup(bot):
    await bot.add_cog(VCJumpscare(bot))