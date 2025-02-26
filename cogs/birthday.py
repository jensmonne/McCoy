import discord
from discord.ext import commands, tasks
import datetime
import json
from config import is_admin_or_owner

class Birthday(commands.Cog):
    def __init__(self, bot):
        self.bot = bot
        self.birthdays = self.load_birthdays()
        self.logged_birthdays = self.load_logged_birthdays()
        self.config = self.load_config()
        self.check_birthdays.start()

    def load_birthdays(self):
        try:
            with open("birthdays.json", "r") as f:
                return json.load(f)
        except (FileNotFoundError, json.JSONDecodeError):
            return {}
    
    def load_logged_birthdays(self):
        try:
            with open("logged_birthdays.json", "r") as f:
                return json.load(f)
        except (FileNotFoundError, json.JSONDecodeError):
            return {}

    def load_config(self):
        try:
            with open("config.json", "r") as f:
                return json.load(f)
        except (FileNotFoundError, json.JSONDecodeError):
            return {}

    def save_birthdays(self):
        with open("birthdays.json", "w") as f:
            json.dump(self.birthdays, f, indent=4)
    
    def save_logged_birthdays(self):
        with open("logged_birthdays.json", "w") as f:
            json.dump(self.logged_birthdays, f, indent=4)

    def save_config(self):
        with open("config.json", "w") as f:
            json.dump(self.config, f, indent=4)

    @commands.command(name="setbirthday", aliases=["sbd"], case_insensitive=True)
    async def set_birthday(self, ctx, date: str):
        try:
            if len(date) == 10:
                birthday = datetime.datetime.strptime(date, "%Y-%m-%d").date()
                display_date = birthday.strftime("%Y-%m-%d")
            elif len(date) == 5:
                birthday = datetime.datetime.strptime(f"{datetime.datetime.now().year}-{date}", "%Y-%m-%d").date()
                display_date = birthday.strftime("%m-%d")
            else:
                raise ValueError
            self.birthdays[str(ctx.author.id)] = date
            self.save_birthdays()
            await ctx.send(f"Birthday set for {ctx.author.mention} on {display_date}!")
        except ValueError:
            await ctx.send("Invalid date format! Use YYYY-MM-DD or MM-DD.")

    @commands.command(name="birthday", aliases=["bd"], case_insensitive=True)
    async def my_birthday(self, ctx):
        birthday = self.birthdays.get(str(ctx.author.id))
        if birthday:
            await ctx.send(f"Your birthday is on {birthday}!")
        else:
            await ctx.send("You haven't set your birthday yet! Use `!set_birthday YYYY-MM-DD` or `!set_birthday MM-DD`.")
    
    @commands.command(name="setbirthdaychannel", aliases=["setbdchannel", "sbdc"], case_insensitive=True)
    @is_admin_or_owner()
    async def set_birthday_channel(self, ctx, channel: discord.TextChannel = None):
        if channel is None:
            await ctx.send("Please mention a channel to set as the birthday channel.")
            return

        guild_id = str(ctx.guild.id)

        if guild_id not in self.config:
            self.config[guild_id] = {}

        self.config[guild_id]["birthday_channel"] = str(channel.id)
        self.save_config()

        await ctx.send(f"Birthday messages will now be sent in {channel.mention}")

    @tasks.loop(hours=24)
    async def check_birthdays(self):
        today = datetime.date.today()
        today_str = today.strftime("%Y-%m-%d")
        today_md = today.strftime("%m-%d")
        for user_id, birthday in self.birthdays.items():
            if birthday.endswith(today_md) and self.logged_birthdays.get(user_id) != today_str:
                user = self.bot.get_user(int(user_id))
                if user:
                    guild_id = str(user.guild.id)
                    channel_id = self.config.get(guild_id, {}).get("birthday_channel")
                    if channel_id:
                        birthday_channel = self.bot.get_channel(int(channel_id))
                    else:
                        birthday_channel = None

                    if len(birthday) == 10:
                        birth_year = int(birthday[:4])
                        age = today.year - birth_year
                        message = f"Happy {age}th Birthday, {user.mention}! 🎉🎂"
                    else:
                        message = f"Happy Birthday, {user.mention}! 🎉🎂"

                    if birthday_channel:
                        await birthday_channel.send(message)
                    else:
                        await user.send(message)

                self.logged_birthdays[user_id] = today_str
        self.save_logged_birthdays()

    @check_birthdays.before_loop
    async def before_check_birthdays(self):
        await self.bot.wait_until_ready()

# Add the Cog to the bot
async def setup(bot):
    await bot.add_cog(Birthday(bot))