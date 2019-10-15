<template>
  <div class="main">
    <div id="choices">
      <div>
        <span style="font-size:0.8em;color:#909090;">välj ett nyckelord</span>
        <br />
        <span v-for="keyword in keywords" :key="keyword">
          <span v-if="keyword == currentKeyword" class="selectedKeywordOption">{{ keyword }}</span>
          <span v-else class="keywordOption" v-on:click="selectKeyword">{{ keyword }}</span>
        </span>
      </div>
      <div>
        <span style="font-size:0.8em;color:#909090;">välj positivt eller negativt</span>
        <br />

        <span v-if="currentSentiment == 'Negativt'" class="selectedSentimentOption">Negativt</span>
        <span v-else class="sentimentOption" v-on:click="setSentiment">Negativt</span>

        <span v-if="currentSentiment == 'Positivt'" class="selectedSentimentOption">Positivt</span>
        <span v-else class="sentimentOption" v-on:click="setSentiment">Positivt</span>
      </div>
      <div>
        <span style="font-size:0.8em;color:#909090;">välj en månad</span>
        <br />
        <span v-for="yearMonth in yearMonths" :key="yearMonth">
          <span v-if="yearMonth == currentYearMonth" class="selectedYearMonthOption">{{ yearMonth }}</span>
          <span v-else class="yearMonthOption" v-on:click="selectYearMonth">{{ yearMonth }}</span>
        </span>
      </div>
    </div>
    <h4 v-if="currentSentiment == 'Positivt'">
      {{currentKeyword}} i
      <span style="color:#64c570">positiva</span>
      meningar {{currentYearMonth}}
    </h4>
    <h4 v-else>
      {{currentKeyword}} i
      <span style="color:#c56464">negativa</span>
      meningar {{currentYearMonth}}
    </h4>
    <div v-if="viewMode == 'Chart'">
      <canvas id="bar-chart-horizontal"></canvas>
      <span class="float-right" v-if="emptyResult === false">
        <!-- duplication ...... fix.. component..-->
        <span class="viewModeLinkButton" v-on:click="changeViewMode('Chart')">diagram</span> |
        <span class="viewModeLinkButton" v-on:click="changeViewMode('Text')">text</span>
      </span>
    </div>
    <div v-else>
      <span class="float-right" v-if="emptyResult === false">
        <span class="viewModeLinkButton" v-on:click="changeViewMode('Chart')">diagram</span> |
        <span class="viewModeLinkButton" v-on:click="changeViewMode('Text')">text</span>
      </span>
      <div class="textData">
        <div v-if="currentSentiment == 'Positivt'">
          <p v-for="(sentence, index) in positiveSentences" :key="index">
            <strong>Datum:</strong>
            {{sentence.received | moment('YYYY-MM-DD')}}
            <br />
            <strong>Källa:</strong>
            {{sentence.source.url}}
            <br />
            <strong>Artikel:</strong>
            {{sentence.sourceArticleHeader}}
            <br />
            <strong>Länk:</strong>
            <a class="source-article-link" :href="sentence.sourceArticleUrl">länk till artikeln</a>
            <br />
            <strong>Mening:</strong>
            {{sentence.text}}
            <br />
          </p>
        </div>
        <div v-else>
          <p v-for="(sentence, index) in negativeSentences" :key="index">
            <strong>Datum:</strong>
            {{sentence.received | moment('YYYY-MM-DD')}}
            <br />
            <strong>Källa:</strong>
            {{sentence.source.url}}
            <br />
            <strong>Artikel:</strong>
            {{sentence.sourceArticleHeader}}
            <br />
            <strong>Länk:</strong>
            <a class="source-article-link" :href="sentence.sourceArticleUrl">länk till artikeln</a>
            <br />
            <strong>Mening:</strong>
            {{sentence.text}}
            <br />
          </p>
        </div>
      </div>
      <span class="float-right" v-if="emptyResult === false">
        <span class="viewModeLinkButton" v-on:click="changeViewMode('Chart')">diagram</span> |
        <span class="viewModeLinkButton" v-on:click="changeViewMode('Text')">text</span>
      </span>
    </div>
  </div>
</template>

<script lang="ts">
// TODO js => ts + massa refactoring
import axios from "axios";
import { renderBarChart } from "../barchart";
export default {
  data() {
    return {
      keywords: [],
      yearMonths: [],
      currentKeyword: "",
      currentYearMonth: "",
      currentSentiment: "Negativt",
      viewMode: "Chart",
      positiveSentences: [],
      negativeSentences: [],
      emptyResult: false
    };
  },
  methods: {
    changeViewMode: function(mode) {
      this.viewMode = mode;
      this.getSentences();
    },
    selectKeyword: function(event) {
      this.currentKeyword = event.target.innerHTML;
      this.getSentences();
    },
    selectYearMonth: function(event) {
      this.currentYearMonth = event.target.innerHTML;
      this.getSentences();
    },
    setSentiment: function(event) {
      this.currentSentiment = event.target.innerHTML;
      this.getSentences();
    },
    getSentences: async function() {
      await axios
        .post("/api/index/sentences", {
          keyword: this.currentKeyword,
          date: this.currentYearMonth + "-01"
        })
        .then(async response => {
          console.log(response);
          await this.visualize(response.data);
        })
        .catch(function(error) {
          console.log(error);
        });
    },
    addToMap: async function(map, s) {
      if (map.has(s.source.url)) {
        let count = map.get(s.source.url);
        count++;
        map.set(s.source.url, count);
      } else {
        map.set(s.source.url, 1);
      }
    },
    getAllKeywords: async function() {
      await axios
        .get("/api/index/allkeywords")
        .then(res => {
          let keywords = res.data;
          this.keywords = keywords;
          this.currentKeyword = keywords[0];
        })
        .catch(err => {
          console.log(err);
        });
    },
    getAllYearMonths: async function() {
      await axios
        .get("/api/index/allyearmonths")
        .then(res => {
          let yearMonths = res.data;
          this.yearMonths = yearMonths;
          this.currentYearMonth = yearMonths[0];
        })
        .catch(err => {
          console.log(err);
        });
    },
    visualize: async function(sentences) {
      let positiveMap = new Map();
      this.positiveSentences = [];
      let negativeMap = new Map();
      this.negativeSentences = [];

      sentences.forEach(s => {
        if (s.positive === true) {
          this.addToMap(positiveMap, s);
          this.positiveSentences.push(s);
        } else {
          this.addToMap(negativeMap, s);
          this.negativeSentences.push(s);
        }
      });
      
        // when porting to ts also clean up all ugly duplication
        if (this.currentSentiment === "Positivt") {
          const positiveSet = {
            labels: [...positiveMap.keys()],
            datapoints: [...positiveMap.values()]
          };
          if (
            positiveSet.labels.length === 0 &&
            positiveSet.datapoints.length === 0
          ) {
            this.emptyResult = true;
          } else {
            this.emptyResult = false;
          }
          renderBarChart(positiveSet);
        } else {
          const negativeSet = {
            labels: [...negativeMap.keys()],
            datapoints: [...negativeMap.values()]
          };
          if (
            negativeSet.labels.length === 0 &&
            negativeSet.datapoints.length === 0
          ) {
            this.emptyResult = true;
          } else {
            this.emptyResult = false;
          }
          renderBarChart(negativeSet);
        }
      
    }
  },
  async mounted() {
    await this.getAllKeywords();
    await this.getAllYearMonths();
    await this.getSentences();
  }
};
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
.source-article-link {margin-left:2px; color:#3e95cd;}
</style>
