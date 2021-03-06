﻿/**
* Copyright 2015 IBM Corp. All Rights Reserved.
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
*      http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*
*/

using UnityEngine;
using System.Collections;
using IBM.Watson.DeveloperCloud.Services.RetrieveAndRank.v1;
using System;
using IBM.Watson.DeveloperCloud.Logging;
using System.IO;
using IBM.Watson.DeveloperCloud.Utilities;
using System.Collections.Generic;

namespace IBM.Watson.DeveloperCloud.UnitTests
{
    public class TestRetrieveAndRank : UnitTest
    {
        RetrieveAndRank m_RetrieveAndRank = new RetrieveAndRank();

        /// <summary>
        /// Enables full test of newly created configs and rankers.
        /// </summary>
        public bool IsFullTest = false;

        private bool m_GetClustersTested = false;
        private bool m_CreateClusterTested = false;
        private bool m_DeleteClusterTested = false;
        private bool m_GetClusterTested = false;
        private bool m_ListClusterConfigsTested = false;
        private bool m_DeleteClusterConfigTested = false;
        private bool m_GetClusterConfigTested = false;
        private bool m_UploadClusterConfigTested = false;
        private bool m_ListCollectionRequestTested = false;
        private bool m_CreateCollectionRequestTested = false;
        private bool m_DeleteCollectionRequestTested = false;
        private bool m_IndexDocumentsTested = false;
        private bool m_StandardSearchTested = false;
        private bool m_RankedSearchTested = false;
        private bool m_GetRankersTested = false;
        private bool m_CreateRankerTested = false;
        private bool m_RankTested = false;
        private bool m_DeleteRankersTested = false;
        private bool m_GetRankerInfoTested = false;

        private string m_ClusterToCreateName = "unity-integration-test-cluster";
        private string m_CreatedClusterID;
        private string m_ConfigToCreateName = "unity-integration-test-config";
        private string m_CollectionToCreateName = "unity-integration-test-collection";
        private string m_RankerToCreateName = "unity-integration-test-ranker";
        private string m_CreatedRankerID;

        //  from config variables
        private string m_ExampleClusterID;
        private string m_ExampleConfigName;
        private string m_ExampleRankerID;
        private string m_ExampleCollectionName;

        private string m_IntegrationTestQuery = "What is the basic mechanisim of the transonic aileron buzz";

        private string[] m_Fl = { "title", "id", "body", "author", "bibliography" };

        //  data paths to local files
        private string m_IntegrationTestClusterConfigPath;
        private string m_IntegrationTestRankerTrainingPath;
        private string m_IntegrationTestRankerAnswerDataPath;
        private string m_IntegrationTestIndexDataPath;

        private bool m_IsClusterReady = false;
        private bool m_IsRankerReady = false;
        public override IEnumerator RunTest()
        {
            m_IntegrationTestClusterConfigPath = Application.dataPath + "/Watson/Examples/ServiceExamples/TestData/RetrieveAndRank/cranfield_solr_config.zip";
            m_IntegrationTestRankerTrainingPath = Application.dataPath + "/Watson/Examples/ServiceExamples/TestData/RetrieveAndRank/ranker_training_data.csv";
            m_IntegrationTestRankerAnswerDataPath = Application.dataPath + "/Watson/Examples/ServiceExamples/TestData/RetrieveAndRank/ranker_answer_data.csv";
            m_IntegrationTestIndexDataPath = Application.dataPath + "/Watson/Examples/ServiceExamples/TestData/RetrieveAndRank/cranfield_data.json";

            m_ExampleClusterID = Config.Instance.GetVariableValue("RetrieveAndRank_IntegrationTestClusterID");
            m_ExampleConfigName = Config.Instance.GetVariableValue("RetrieveAndRank_IntegrationTestConfigName");
            m_ExampleRankerID = Config.Instance.GetVariableValue("RetrieveAndRank_IntegrationTestRankerID");
            m_ExampleCollectionName = Config.Instance.GetVariableValue("RetrieveAndRank_IntegrationTestCollectionName");

            //  Get clusters
            Log.Debug("TestRetrieveAndRank", "*** Attempting to get clusters!");
            m_RetrieveAndRank.GetClusters(OnGetClusters);
            while (!m_GetClustersTested)
                yield return null;
            
            //  Create cluster
            Log.Debug("TestRetrieveAndRank", "*** Attempting to create cluster!");
            m_RetrieveAndRank.CreateCluster(OnCreateCluster, m_ClusterToCreateName, "1");
            while (!m_CreateClusterTested)
                yield return null;
            
            //  Get created cluster
            Log.Debug("TestRetrieveAndRank", "*** Attempting to get created cluster {0}!", IsFullTest ? m_CreatedClusterID : m_ExampleClusterID);
            m_RetrieveAndRank.GetCluster(OnGetCluster, IsFullTest ? m_CreatedClusterID : m_ExampleClusterID);
            while (!m_GetClusterTested || !m_IsClusterReady)
                yield return null;

            //  List cluster configs
            Log.Debug("TestRetrieveAndRank", "*** Attempting to get cluster configs for {0}!", IsFullTest ? m_CreatedClusterID : m_ExampleClusterID);
            m_RetrieveAndRank.GetClusterConfigs(OnGetClusterConfigs, IsFullTest ? m_CreatedClusterID : m_ExampleClusterID);
            while (!m_ListClusterConfigsTested)
                yield return null;

            //  Upload cluster config
            Log.Debug("TestRetrieveAndRank", "*** Attempting to upload cluster {0} config {1}!", IsFullTest ? m_CreatedClusterID : m_ExampleClusterID, m_ConfigToCreateName);
            m_RetrieveAndRank.UploadClusterConfig(OnUploadClusterConfig, IsFullTest ? m_CreatedClusterID : m_ExampleClusterID, m_ConfigToCreateName, m_IntegrationTestClusterConfigPath);
            while (!m_UploadClusterConfigTested)
                yield return null;

            //  Get cluster config
            Log.Debug("TestRetrieveAndRank", "*** Attempting to get cluster {0} config {1}!", IsFullTest ? m_CreatedClusterID : m_ExampleClusterID, IsFullTest ? m_ConfigToCreateName : m_ExampleConfigName);
            m_RetrieveAndRank.GetClusterConfig(OnGetClusterConfig, IsFullTest ? m_CreatedClusterID : m_ExampleClusterID, IsFullTest ? m_ConfigToCreateName : m_ExampleConfigName);
            while (!m_GetClusterConfigTested)
                yield return null;

            //  Create Collection
            Log.Debug("TestRetrieveAndRank", "*** Attempting to create collection!");
            m_RetrieveAndRank.ForwardCollectionRequest(OnCreateCollections, IsFullTest ? m_CreatedClusterID : m_ExampleClusterID, CollectionsAction.CREATE, m_CollectionToCreateName, IsFullTest ? m_ConfigToCreateName : m_ExampleConfigName);
            while (!m_CreateCollectionRequestTested)
                yield return null;

            //  List Collections
            Log.Debug("TestRetrieveAndRank", "*** Attempting to list collections!");
            m_RetrieveAndRank.ForwardCollectionRequest(OnListCollections, IsFullTest ? m_CreatedClusterID : m_ExampleClusterID, CollectionsAction.LIST);
            while (!m_ListCollectionRequestTested)
                yield return null;

            //  Index documents
            Log.Debug("TestRetrieveAndRank", "*** Attempting to index documents!");
            m_RetrieveAndRank.IndexDocuments(OnIndexDocuments, m_IntegrationTestIndexDataPath, IsFullTest ? m_CreatedClusterID : m_ExampleClusterID, m_CollectionToCreateName);
            while (!m_IndexDocumentsTested)
                yield return null;

            //  Get rankers
            Log.Debug("TestRetrieveAndRank", "*** Attempting to get rankers!");
            m_RetrieveAndRank.GetRankers(OnGetRankers);
            while (!m_GetRankersTested)
                yield return null;
            
            //  Create ranker
            Log.Debug("TestRetrieveAndRank", "*** Attempting to create rankers!");
            m_RetrieveAndRank.CreateRanker(OnCreateRanker, m_IntegrationTestRankerTrainingPath, m_RankerToCreateName);
            while (!m_CreateRankerTested)
                yield return null;

            //  Get ranker info
            Log.Debug("TestRetrieveAndRank", "*** Attempting to get Ranker Info!");
            m_RetrieveAndRank.GetRanker(OnGetRanker, IsFullTest ? m_CreatedRankerID : m_ExampleRankerID);
            while (!m_GetRankerInfoTested || !m_IsRankerReady)
                yield return null;

            //  Rank
            Log.Debug("TestRetrieveAndRank", "*** Attempting to rank!");
            m_RetrieveAndRank.Rank(OnRank, IsFullTest ? m_CreatedRankerID : m_ExampleRankerID, m_IntegrationTestRankerAnswerDataPath);
            while (!m_RankTested)
                yield return null;
            
            //  Standard Search
            Log.Debug("TestRetrieveAndRank", "*** Attempting to search!");
            m_RetrieveAndRank.Search(OnStandardSearch, IsFullTest ? m_CreatedClusterID : m_ExampleClusterID, IsFullTest ? m_CollectionToCreateName : m_ExampleCollectionName, m_IntegrationTestQuery, m_Fl);
            while (!m_StandardSearchTested)
                yield return null;

            //  Ranked Search
            Log.Debug("TestRetrieveAndRank", "*** Attempting to search!");
            m_RetrieveAndRank.Search(OnRankedSearch, IsFullTest ? m_CreatedClusterID : m_ExampleClusterID, IsFullTest ? m_CollectionToCreateName : m_ExampleCollectionName, m_IntegrationTestQuery, m_Fl, true, m_ExampleRankerID);
            while (!m_RankedSearchTested)
                yield return null;
            
            //  Delete ranker
            Log.Debug("ExampleRetriveAndRank", "*** Attempting to delete ranker {0}!", m_RankerToCreateName);
            m_RetrieveAndRank.DeleteRanker(OnDeleteRanker, m_CreatedRankerID);
            while (!m_DeleteRankersTested)
                yield return null;

            //  Delete Collection request
            Log.Debug("TestRetrieveAndRank", "*** Attempting to delete collection!");
            m_RetrieveAndRank.ForwardCollectionRequest(OnDeleteCollections, IsFullTest ? m_CreatedClusterID : m_ExampleClusterID, CollectionsAction.DELETE, m_CollectionToCreateName);
            while (!m_DeleteCollectionRequestTested)
                yield return null;

            //  Delete cluster config
            Log.Debug("TestRetrieveAndRank", "** Attempting to delete config {1} from cluster {0}!", IsFullTest ? m_CreatedClusterID : m_ExampleClusterID, m_ConfigToCreateName);
            m_RetrieveAndRank.DeleteClusterConfig(OnDeleteClusterConfig, IsFullTest ? m_CreatedClusterID : m_ExampleClusterID, m_ConfigToCreateName);
            while (!m_DeleteClusterConfigTested)
                yield return null;

            yield return new WaitForSeconds(10f);

            //  Delete cluster
            Log.Debug("TestRetrieveAndRank", "*** Attempting to delete cluster {0}!", m_CreatedClusterID);
            m_RetrieveAndRank.DeleteCluster(OnDeleteCluster, m_CreatedClusterID);
            while (!m_DeleteClusterTested)
                yield return null;

            yield break;
        }


        private void OnGetClusters(SolrClusterListResponse resp, string data)
        {
            Test(resp != null);

            if (resp != null)
                foreach (SolrClusterResponse cluster in resp.clusters)
                    Log.Debug("TestRetrieveAndRank", "OnGetClusters | cluster name: {0}, size: {1}, ID: {2}, status: {3}.", cluster.cluster_name, cluster.cluster_size, cluster.solr_cluster_id, cluster.solr_cluster_status);
            else
                Log.Debug("TestRetrieveAndRank", "OnGetClusters | Get Cluster Response is null!");

            m_GetClustersTested = true;
        }

        private void OnCreateCluster(SolrClusterResponse resp, string data)
        {
            Test(resp != null);

            if (resp != null)
            {
                Log.Debug("TestRetrieveAndRank", "OnCreateCluster | name: {0}, size: {1}, ID: {2}, status: {3}.", resp.cluster_name, resp.cluster_size, resp.solr_cluster_id, resp.solr_cluster_status);
                m_CreatedClusterID = resp.solr_cluster_id;
            }
            else
                Log.Debug("TestRetrieveAndRank", "OnCreateCluster | Get Cluster Response is null!");

            m_CreateClusterTested = true;
        }

        private void OnGetCluster(SolrClusterResponse resp, string data)
        {
            Test(resp != null);

            if (resp != null)
            {
                Log.Debug("TestRetrieveAndRank", "OnGetCluster | name: {0}, size: {1}, ID: {2}, status: {3}.", resp.cluster_name, resp.cluster_size, resp.solr_cluster_id, resp.solr_cluster_status);

                if (resp.solr_cluster_status != "READY")
                    m_RetrieveAndRank.GetCluster(OnGetCluster, IsFullTest ? m_CreatedClusterID : m_ExampleClusterID);
                else
                    m_IsClusterReady = true;
            }
            else
                Log.Debug("TestRetrieveAndRank", "OnGetCluster | Get Cluster Response is null!");

            m_GetClusterTested = true;
        }

        private void OnDeleteCluster(bool success, string data)
        {
            Test(success);

            if (success)
                Log.Debug("TestRetrieveAndRank", "OnDeleteCluster | Success!");
            else
                Log.Debug("TestRetrieveAndRank", "OnDeleteCluster | Failure!");

            m_DeleteClusterTested = true;
        }

        private void OnGetClusterConfigs(SolrConfigList resp, string data)
        {
            Test(resp != null);

            if (resp != null)
            {
                if (resp.solr_configs.Length == 0)
                    Log.Debug("TestRetrieveAndRank", "OnGetClusterConfigs | no cluster configs!");

                foreach (string config in resp.solr_configs)
                    Log.Debug("TestRetrieveAndRank", "OnGetClusterConfigs | solr_config: " + config);
            }
            else
                Log.Debug("TestRetrieveAndRank", "OnGetClustersConfigs | Get Cluster Configs Response is null!");

            m_ListClusterConfigsTested = true;
        }

        private void OnUploadClusterConfig(UploadResponse resp, string data)
        {
            Test(resp != null);

            if (resp != null)
                Log.Debug("TestRetrieveAndRank", "OnUploadClusterConfig | Success! {0}, {1}", resp.message, resp.statusCode);
            else
                Log.Debug("TestRetrieveAndRank", "OnUploadClusterConfig | Failure!");

            m_UploadClusterConfigTested = true;
        }

        private void OnDeleteClusterConfig(bool success, string data)
        {
            Test(success);

            if (success)
                Log.Debug("TestRetrieveAndRank", "OnDeleteClusterConfig | Success!");
            else
                Log.Debug("TestRetrieveAndRank", "OnDeleteClusterConfig | Failure!");

            m_DeleteClusterConfigTested = true;
        }

        private void OnGetClusterConfig(byte[] respData, string data)
        {
            Test(respData != null);

            if (respData != null)
            {
                Log.Debug("TestRetrieveAndRank", "OnGetClusterConfig | success!");
            }
            else
                Log.Debug("TestRetrieveAndRank", "OnGetClusterConfig | respData is null!");

            m_GetClusterConfigTested = true;
        }

        private void OnCreateCollections(CollectionsResponse resp, string data)
        {
            Test(resp != null);

            if (resp != null)
            {
                if (resp.responseHeader != null)
                    Log.Debug("TestRetrieveAndRank", "OnCreateCollections | status: {0}, QTime: {1}.", resp.responseHeader.status, resp.responseHeader.QTime);
                if (resp.collections != null)
                {
                    if (resp.collections.Length == 0)
                        Log.Debug("TestRetrieveAndRank", "OnCreateCollections | There are no collections!");
                    else
                        foreach (string collection in resp.collections)
                            Log.Debug("TestRetrieveAndRank", "\tOnCreateCollections | collection: {0}", collection);
                }
            }
            else
                Log.Debug("TestRetrieveAndRank", "OnCreateCollections | GetCollections Response is null!");

            m_CreateCollectionRequestTested = true;
        }

        private void OnListCollections(CollectionsResponse resp, string data)
        {
            Test(resp != null);

            if (resp != null)
            {
                if (resp.responseHeader != null)
                    Log.Debug("TestRetrieveAndRank", "OnListCollections | status: {0}, QTime: {1}.", resp.responseHeader.status, resp.responseHeader.QTime);
                if (resp.collections != null)
                {
                    if (resp.collections.Length == 0)
                        Log.Debug("TestRetrieveAndRank", "OnListCollections | There are no collections!");
                    else
                        foreach (string collection in resp.collections)
                        {
                            Log.Debug("TestRetrieveAndRank", "\tOnListCollections | collection: {0}", collection);
                            
                            if (collection == (IsFullTest ? m_CollectionToCreateName : m_ExampleCollectionName))
                                Log.Debug("TestRetrieveAndRank", "\t\tOnListCollections | created collection found!: {0}", collection);
                            else
                                Log.Debug("TestRetrieveAndRank", "\t\tOnListCollections | created collection not found!: {0}", collection);
                        }
                }
            }
            else
                Log.Debug("TestRetrieveAndRank", "OnListCollections | GetCollections Response is null!");

            m_ListCollectionRequestTested = true;
        }

        private void OnDeleteCollections(CollectionsResponse resp, string data)
        {
            Test(resp != null);

            if (resp != null)
            {
                if (resp.responseHeader != null)
                    Log.Debug("TestRetrieveAndRank", "OnDeleteCollections | status: {0}, QTime: {1}.", resp.responseHeader.status, resp.responseHeader.QTime);
                if (resp.collections != null)
                {
                    if (resp.collections.Length == 0)
                        Log.Debug("TestRetrieveAndRank", "OnDeleteCollections | There are no collections!");
                    else
                        foreach (string collection in resp.collections)
                            Log.Debug("TestRetrieveAndRank", "\tOnDeleteCollections | collection: {0}", collection);
                }
            }
            else
                Log.Debug("TestRetrieveAndRank", "OnDeleteCollections | GetCollections Response is null!");

            m_DeleteCollectionRequestTested = true;
        }

        private void OnGetRankers(ListRankersPayload resp, string data)
        {
            Test(resp != null);

            if (resp != null)
            {
                if (resp.rankers.Length == 0)
                    Log.Debug("TestRetrieveAndRank", "OnGetRankers | no rankers!");
                foreach (RankerInfoPayload ranker in resp.rankers)
                    Log.Debug("TestRetrieveAndRank", "\tOnGetRankers | ranker name: {0}, ID: {1}, created: {2}, url: {3}.", ranker.name, ranker.ranker_id, ranker.created, ranker.url);
            }
            else
                Log.Debug("TestRetrieveAndRank", "OnGetRankers | Get Ranker Response is null!");

            m_GetRankersTested = true;
        }

        private void OnCreateRanker(RankerStatusPayload resp, string data)
        {
            Test(resp != null);

            if (resp != null)
            {
                Log.Debug("TestRetrieveAndRank", "OnCreateRanker | ID: {0}, url: {1}, name: {2}, created: {3}, status: {4}, statusDescription: {5}.", resp.ranker_id, resp.url, resp.name, resp.created, resp.status, resp.status_description);
                m_CreatedRankerID = resp.ranker_id;
            }
            else
                Log.Debug("TestRetrieveAndRank", "OnCreateRanker | Get Cluster Response is null!");

            m_CreateRankerTested = true;
        }

        private void OnGetRanker(RankerStatusPayload resp, string data)
        {
            Test(resp != null);

            if (resp != null)
            {
                Log.Debug("TestRetrieveAndRank", "GetRanker | ranker_id: {0}, url: {1}, name: {2}, created: {3}, status: {4}, status_description: {5}.", resp.ranker_id, resp.url, resp.name, resp.created, resp.status, resp.status_description);
                if (resp.status != "Available")
                    m_RetrieveAndRank.GetRanker(OnGetRanker, IsFullTest ? m_CreatedRankerID : m_ExampleRankerID);
                else
                    m_IsRankerReady = true;
            }
            else
                Log.Debug("TestRetrieveAndRank", "GetRanker | GetRanker response is null!");

            m_GetRankerInfoTested = true;
        }

        private void OnRank(RankerOutputPayload resp, string data)
        {
            Test(resp != null);

            if (resp != null)
            {
                Log.Debug("TestRetrieveAndRank", "OnRank | ID: {0}, url: {1}, top_answer: {2}.", resp.ranker_id, resp.url, resp.top_answer);
                if (resp.answers != null)
                    if (resp.answers.Length == 0)
                        Log.Debug("TestRetrieveAndRank", "\tThere are no answers!");
                    else
                        foreach (RankedAnswer answer in resp.answers)
                            Log.Debug("TestRetrieveAndRank", "\tOnRank | answerID: {0}, score: {1}, confidence: {2}.", answer.answer_id, answer.score, answer.confidence);
            }
            else
                Log.Debug("TestRetrieveAndRank", "OnRank | Rank response is null!");

            m_RankTested = true;
        }

        private void OnDeleteRanker(bool success, string data)
        {
            Test(success);

            if (success)
            {
                Log.Debug("TestRetrieveAndRank", "OnDeleteRanker | Success!");
            }
            else
            {
                Log.Debug("TestRetrieveAndRank", "OnDeleteRanker | Failure!");
            }

            m_DeleteRankersTested = true;
        }

        private void OnIndexDocuments(IndexResponse resp, string data)
        {
            Test(resp != null);

            if (resp != null)
            {
                if (resp.responseHeader != null)
                    Log.Debug("TestRetrieveAndRank", "OnIndexDocuments | status: {0}, QTime: {1}", resp.responseHeader.status, resp.responseHeader.QTime);
                else
                    Log.Debug("TestRetrieveAndRank", "OnIndexDocuments | Response header is null!");
            }
            else
            {
                Log.Debug("TestRetrieveAndRank", "OnIndexDocuments | response is null!");
            }

            m_IndexDocumentsTested = true;
        }

        private void OnStandardSearch(SearchResponse resp, string data)
        {
            Test(resp != null);

            if (resp != null)
            {
                if (resp.responseHeader != null)
                {
                    Log.Debug("TestRetrieveAndRank", "Search | status: {0}, QTime: {1}.", resp.responseHeader.status, resp.responseHeader.QTime);
                    if (resp.responseHeader._params != null)
                        Log.Debug("TestRetrieveAndRank", "\tSearch | params.q: {0}, params.fl: {1}, params.wt: {2}.", resp.responseHeader._params.q, resp.responseHeader._params.fl, resp.responseHeader._params.wt);
                    else
                        Log.Debug("TestRetrieveAndRank", "Search | responseHeader.params is null!");
                }
                else
                {
                    Log.Debug("TestRetrieveAndRank", "Search | response header is null!");
                }

                if (resp.response != null)
                {
                    Log.Debug("TestRetrieveAndRank", "Search | numFound: {0}, start: {1}.", resp.response.numFound, resp.response.start);
                    if (resp.response.docs != null)
                    {
                        if (resp.response.docs.Length == 0)
                            Log.Debug("TestRetrieveAndRank", "Search | There are no docs!");
                        else
                            foreach (Doc doc in resp.response.docs)
                            {
                                Log.Debug("TestRetrieveAndRank", "\tSearch | id: {0}.", doc.id);

                                if (doc.title != null)
                                {
                                    if (doc.title.Length == 0)
                                        Log.Debug("TestRetrieveAndRank", "Search | There are no title");
                                    else
                                        foreach (string s in doc.title)
                                            Log.Debug("TestRetrieveAndRank", "\tSearch | title: {0}.", s);
                                }
                                else
                                {
                                    Log.Debug("TestRetrieveAndRank", "Search | title is null");
                                }

                                if (doc.author != null)
                                {
                                    if (doc.author.Length == 0)
                                        Log.Debug("TestRetrieveAndRank", "Search | There are no authors");
                                    else
                                        foreach (string s in doc.author)
                                            Log.Debug("TestRetrieveAndRank", "\tSearch | Author: {0}.", s);
                                }
                                else
                                {
                                    Log.Debug("TestRetrieveAndRank", "Search | Authors is null");
                                }

                                if (doc.body != null)
                                {
                                    if (doc.body.Length == 0)
                                        Log.Debug("TestRetrieveAndRank", "Search | There are no body");
                                    else
                                        foreach (string s in doc.body)
                                            Log.Debug("TestRetrieveAndRank", "\tSearch | body: {0}.", s);
                                }
                                else
                                {
                                    Log.Debug("TestRetrieveAndRank", "Search | Body is null");
                                }

                                if (doc.bibliography != null)
                                {
                                    if (doc.bibliography.Length == 0)
                                        Log.Debug("TestRetrieveAndRank", "Search | There are no bibliographies");
                                    else
                                        foreach (string s in doc.bibliography)
                                            Log.Debug("TestRetrieveAndRank", "\tSearch | bibliography: {0}.", s);
                                }
                                else
                                {
                                    Log.Debug("TestRetrieveAndRank", "Search | Bibliography is null");
                                }
                            }
                    }
                    else
                    {
                        Log.Debug("TestRetrieveAndRank", "Search | docs are null!");
                    }
                }
                else
                {
                    Log.Debug("TestRetrieveAndRank", "Search | response is null!");
                }
            }
            else
            {
                Log.Debug("TestRetrieveAndRank", "Search response is null!");
            }

            m_StandardSearchTested = true;
        }

        private void OnRankedSearch(SearchResponse resp, string data)
        {
            Test(resp != null);

            if (resp != null)
            {
                if (resp.responseHeader != null)
                {
                    Log.Debug("TestRetrieveAndRank", "Search | status: {0}, QTime: {1}.", resp.responseHeader.status, resp.responseHeader.QTime);
                    if (resp.responseHeader._params != null)
                        Log.Debug("TestRetrieveAndRank", "\tSearch | params.q: {0}, params.fl: {1}, params.wt: {2}.", resp.responseHeader._params.q, resp.responseHeader._params.fl, resp.responseHeader._params.wt);
                    else
                        Log.Debug("TestRetrieveAndRank", "Search | responseHeader.params is null!");
                }
                else
                {
                    Log.Debug("TestRetrieveAndRank", "Search | response header is null!");
                }

                if (resp.response != null)
                {
                    Log.Debug("TestRetrieveAndRank", "Search | numFound: {0}, start: {1}.", resp.response.numFound, resp.response.start);
                    if (resp.response.docs != null)
                    {
                        if (resp.response.docs.Length == 0)
                            Log.Debug("TestRetrieveAndRank", "Search | There are no docs!");
                        else
                            foreach (Doc doc in resp.response.docs)
                            {
                                Log.Debug("TestRetrieveAndRank", "\tSearch | id: {0}.", doc.id);

                                if (doc.title != null)
                                {
                                    if (doc.title.Length == 0)
                                        Log.Debug("TestRetrieveAndRank", "Search | There are no title");
                                    else
                                        foreach (string s in doc.title)
                                            Log.Debug("TestRetrieveAndRank", "\tSearch | title: {0}.", s);
                                }
                                else
                                {
                                    Log.Debug("TestRetrieveAndRank", "Search | title is null");
                                }

                                if (doc.author != null)
                                {
                                    if (doc.author.Length == 0)
                                        Log.Debug("TestRetrieveAndRank", "Search | There are no authors");
                                    else
                                        foreach (string s in doc.author)
                                            Log.Debug("TestRetrieveAndRank", "\tSearch | Author: {0}.", s);
                                }
                                else
                                {
                                    Log.Debug("TestRetrieveAndRank", "Search | Authors is null");
                                }

                                if (doc.body != null)
                                {
                                    if (doc.body.Length == 0)
                                        Log.Debug("TestRetrieveAndRank", "Search | There are no body");
                                    else
                                        foreach (string s in doc.body)
                                            Log.Debug("TestRetrieveAndRank", "\tSearch | body: {0}.", s);
                                }
                                else
                                {
                                    Log.Debug("TestRetrieveAndRank", "Search | Body is null");
                                }

                                if (doc.bibliography != null)
                                {
                                    if (doc.bibliography.Length == 0)
                                        Log.Debug("TestRetrieveAndRank", "Search | There are no bibliographies");
                                    else
                                        foreach (string s in doc.bibliography)
                                            Log.Debug("TestRetrieveAndRank", "\tSearch | bibliography: {0}.", s);
                                }
                                else
                                {
                                    Log.Debug("TestRetrieveAndRank", "Search | Bibliography is null");
                                }
                            }
                    }
                    else
                    {
                        Log.Debug("TestRetrieveAndRank", "Search | docs are null!");
                    }
                }
                else
                {
                    Log.Debug("TestRetrieveAndRank", "Search | response is null!");
                }
            }
            else
            {
                Log.Debug("TestRetrieveAndRank", "Search response is null!");
            }

            m_RankedSearchTested = true;
        }
    }
}
