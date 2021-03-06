﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Releaser.Core.Entities;
using Releaser.Core.EntityStore;
using Releaser.Core.EventStore;

// ReSharper disable PossibleNullReferenceException
namespace Releaser.Migrations
{
	public class Program
	{
		private static readonly Dictionary<string, string> s_userMap = new Dictionary<string, string>();
		private static readonly Dictionary<string, string> s_releasesMap = new Dictionary<string, string>();
		private static readonly Dictionary<string, string> s_configurationMap = new Dictionary<string, string>();

		private static FileEntityStore s_entityStore;
		private static FileEventStore s_eventStore;

		public static void Main()
		{
			s_entityStore = new FileEntityStore(@"C:\Temp\ReleaserData");
			s_eventStore = new FileEventStore(@"C:\Temp\ReleaserData\EventStore.dat");

			LoadUsers();
			LoadProjects();
			LoadKeywords();
			LoadReleasers();
			LoadReleases();
			LoadConfigurations();
			LoadDeployers();
			LoadDeployments();

			Console.WriteLine();
			Console.WriteLine("Press any key to continue...");
			Console.ReadKey();
		}

		private static void LoadUsers()
		{
			Console.WriteLine("Loading users...");

			var sw = new Stopwatch();
			sw.Start();

			string dataFile = File.ReadAllText(@"..\..\..\Data.tmp\ReleaserData.xml");

			var xml = XElement.Parse(dataFile);

			var userNodes = xml.Elements("User").ToList();
			foreach (XElement userNode in userNodes)
			{
				var user = new User(
					userNode.Element("UserCode").Value,
					userNode.Element("UserLogin").Value,
					userNode.Element("UserName").Value);

				s_entityStore.Write(user);
				s_eventStore.SaveEvents(user.GetChanges());

				s_userMap.Add(
					userNode.Element("UserUid").Value,
					userNode.Element("UserCode").Value);
			}

			sw.Stop();

			Console.WriteLine(
				"{0} users were loaded ({1}ms).",
				userNodes.Count,
				sw.ElapsedMilliseconds);
		}

		private static void LoadProjects()
		{
			Console.WriteLine("Loading projects...");

			var sw = new Stopwatch();
			sw.Start();

			string dataFile = File.ReadAllText(@"..\..\..\Data.tmp\ReleaserData.xml");

			var xml = XElement.Parse(dataFile);

			var projectNodes = xml.Elements("Project").ToList();
			foreach (XElement projectNode in projectNodes)
			{
				var project = new Project(
					projectNode.Element("ProjectName").Value,
					projectNode.Element("StoragePath").Value,
					projectNode.Element("StorageCode").Value,
					projectNode.Element("ImageCode").Value);

				project.Id = projectNode.Element("ProjectUid").Value;

				s_entityStore.Write(project);
				s_eventStore.SaveEvents(project.GetChanges());
			}

			sw.Stop();

			Console.WriteLine(
				"{0} projects were loaded ({1}ms).",
				projectNodes.Count,
				sw.ElapsedMilliseconds);
		}

		private static void LoadKeywords()
		{
			Console.WriteLine("Loading keywords...");

			var sw = new Stopwatch();
			sw.Start();

			string dataFile = File.ReadAllText(@"..\..\..\Data.tmp\ReleaserData.xml");

			var xml = XElement.Parse(dataFile);

			Dictionary<string, string> keywordMap = new Dictionary<string, string>();

			var keywords = xml.Elements("Keyword").ToList();
			foreach (XElement keyword in keywords)
			{
				keywordMap.Add(
					keyword.Element("KeywordUid").Value,
					keyword.Element("Keyword").Value);
			}

			Dictionary<string, List<string>> projectKeywordData = new Dictionary<string, List<string>>();

			var projectKeywords = xml.Elements("ProjectKeyword").ToList();
			foreach (XElement keyword in projectKeywords)
			{
				var projectId = keyword.Element("ProjectUid").Value;
				if (!projectKeywordData.ContainsKey(projectId))
					projectKeywordData[projectId] = new List<string>();

				projectKeywordData[projectId].Add(
					keywordMap[keyword.Element("KeywordUid").Value]);
			}

			foreach (var projectId in projectKeywordData.Keys)
			{
				var project = s_entityStore.Read<Project>(projectId);
				project.AddKeywords(projectKeywordData[projectId]);

				s_entityStore.Write(project);
				s_eventStore.SaveEvents(project.GetChanges());
			}

			sw.Stop();

			Console.WriteLine(
				"{0} keywords were loaded ({1}ms).",
				projectKeywords.Count,
				sw.ElapsedMilliseconds);
		}

		private static void LoadReleasers()
		{
			Console.WriteLine("Loading releasers...");

			var sw = new Stopwatch();
			sw.Start();

			string dataFile = File.ReadAllText(@"..\..\..\Data.tmp\ReleaserData.xml");

			var xml = XElement.Parse(dataFile);

			var releasers = xml.Elements("Releaser").ToList();
			foreach (XElement releaser in releasers)
			{
				var projectId = releaser.Element("ProjectUid").Value;
				var project = s_entityStore.Read<Project>(projectId);
				project.AddReleaser(s_userMap[releaser.Element("UserUid").Value]);

				s_entityStore.Write(project);
				s_eventStore.SaveEvents(project.GetChanges());
			}

			sw.Stop();

			Console.WriteLine(
				"{0} releasers were loaded ({1}ms).",
				releasers.Count,
				sw.ElapsedMilliseconds);
		}

		private static void LoadReleases()
		{
			Console.WriteLine("Loading releases...");

			var sw = new Stopwatch();
			sw.Start();

			string dataFile = File.ReadAllText(@"..\..\..\Data.tmp\ReleaserData.xml");

			var xml = XElement.Parse(dataFile);

			var releaseNodes = xml.Elements("Release").ToList();
			foreach (XElement releaseNode in releaseNodes)
			{
				var release = new Release(
					releaseNode.Element("ReleaseCode").Value,
					releaseNode.Element("VersionCode").Value,
					releaseNode.Element("ProjectUid").Value,
					s_userMap[releaseNode.Element("UserUid").Value],
					releaseNode.Element("ReleaseComment").Value);

				release.ReleaseDate = DateTime.Parse(releaseNode.Element("ReleaseDate").Value);

				s_entityStore.Write(release);
				s_eventStore.SaveEvents(release.GetChanges());

				s_releasesMap.Add(
					releaseNode.Element("ReleaseUid").Value,
					releaseNode.Element("ReleaseCode").Value);
			}

			sw.Stop();

			Console.WriteLine(
				"{0} releases were loaded ({1}ms).",
				releaseNodes.Count,
				sw.ElapsedMilliseconds);
		}

		private static void LoadConfigurations()
		{
			Console.WriteLine("Loading configurations...");

			var sw = new Stopwatch();
			sw.Start();

			var configuration = new Configuration(
				"C12",
				"C12 Configuration",
				"Configuration in C12 datacenter");

			s_entityStore.Write(configuration);
			s_eventStore.SaveEvents(configuration.GetChanges());

			s_configurationMap.Add("56d8ea93-95af-4f54-8d81-5fbcf1768550", "C12");

			configuration = new Configuration(
				"LIVE (PHX2)",
				"LIVE (PHX2) Configuration",
				"LIVE configuration in PHOENIX datacenter");

			s_entityStore.Write(configuration);
			s_eventStore.SaveEvents(configuration.GetChanges());

			s_configurationMap.Add("3a23c70a-b5dc-4883-a825-75bd2a639a46", "LIVE (PHX2)");

			sw.Stop();

			Console.WriteLine(
				"{0} configurations were loaded ({1}ms).",
				2,
				sw.ElapsedMilliseconds);
		}

		private static void LoadDeployers()
		{
			Console.WriteLine("Loading deployers...");

			var sw = new Stopwatch();
			sw.Start();

			string dataFile = File.ReadAllText(@"..\..\..\Data.tmp\ReleaserData.xml");

			var xml = XElement.Parse(dataFile);

			var deployers = xml.Elements("Deployer").ToList();
			foreach (XElement deployer in deployers)
			{
				var configurationId = deployer.Element("ConfigurationUid").Value;
				var configuration = s_entityStore.Read<Configuration>(s_configurationMap[configurationId]);
				configuration.AddDeployer(s_userMap[deployer.Element("UserUid").Value]);

				s_entityStore.Write(configuration);
				s_eventStore.SaveEvents(configuration.GetChanges());
			}

			sw.Stop();

			Console.WriteLine(
				"{0} deployers were loaded ({1}ms).",
				deployers.Count,
				sw.ElapsedMilliseconds);
		}

		private static void LoadDeployments()
		{
			Console.WriteLine("Loading deployments...");

			var sw = new Stopwatch();
			sw.Start();

			string dataFile = File.ReadAllText(@"..\..\..\Data.tmp\ReleaserData.xml");

			var xml = XElement.Parse(dataFile);

			var deploymentNodes = xml.Elements("Deployment").ToList();
			foreach (XElement deploymentNode in deploymentNodes)
			{
				var deployment = new Deployment(
					s_releasesMap[deploymentNode.Element("ReleaseUid").Value],
					s_configurationMap[deploymentNode.Element("ConfigurationUid").Value],
					s_userMap[deploymentNode.Element("UserUid").Value]);

				deployment.Id = deploymentNode.Element("DeploymentUid").Value;
				deployment.CreationDate = DateTime.Parse(deploymentNode.Element("DeploymentDate").Value);

				s_entityStore.Write(deployment);
				s_eventStore.SaveEvents(deployment.GetChanges());
			}

			sw.Stop();

			Console.WriteLine(
				"{0} deployments were loaded ({1}ms).",
				deploymentNodes.Count,
				sw.ElapsedMilliseconds);
		}
	}

	// ReSharper restore PossibleNullReferenceException
}
