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
        <span style="font-size:0.8em;color:#909090;">välj en månad</span>
        <br />

        <span v-if="currentSentiment == 'Negativt'" class="selectedSentimentOption">Negativt</span>
        <span v-else class="sentimentOption" v-on:click="setSentiment">Negativt</span>

        <span v-if="currentSentiment == 'Positivt'" class="selectedSentimentOption">Positivt</span>
        <span v-else class="sentimentOption" v-on:click="setSentiment">Positivt</span>
      </div>
      <div>
        <span style="font-size:0.8em;color:#909090;">välj positivt eller negativt</span>
        <br />
        <span v-for="yearMonth in yearMonths" :key="yearMonth">
          <span v-if="yearMonth == currentYearMonth" class="selectedYearMonthOption">{{ yearMonth }}</span>
          <span v-else class="yearMonthOption" v-on:click="selectYearMonth">{{ yearMonth }}</span>
        </span>
      </div>
    </div>
    <h4 v-if="currentSentiment == 'Positivt'">
      {{currentKeyword}}
      <span style="color:#64c570">positiva</span>
      meningar {{currentYearMonth}}
    </h4>
    <h4 v-else>
      {{currentKeyword}}
      <span style="color:#c56464">negativa</span>
      meningar {{currentYearMonth}}
    </h4>
    <div>
      <canvas id="bar-chart-horizontal"></canvas>
    </div>
  </div>
</template>

<script lang="ts">
// TODO js => ts
import axios from "axios";
export default {
  data() {
    return {
      keywords: [],
      yearMonths: [],
      currentKeyword: "",
      currentYearMonth: "",
      currentSentiment: "Negativt"
    };
  },
  methods: {
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
    getSentences: async function(event) {
      await axios
        .post(
          "/api/index/sentences",
          {
            keyword: this.currentKeyword,
            date: this.currentYearMonth + "-01"
          }
        )
        .then(function(response) {
          console.log(response);
        })
        .catch(function(error) {
          console.log(error);
        });
    }
  },
  mounted() {
    console.log("test");
    axios
      .get("/api/index/allkeywords")
      .then(res => {
        let keywords = res.data;
        console.log(keywords);
        this.keywords = keywords;
        this.currentKeyword = keywords[0];
      })
      .catch(err => {
        console.log(err);
      });
    axios
      .get("/api/index/allyearmonths")
      .then(res => {
        let yearMonths = res.data;
        console.log(yearMonths);
        this.yearMonths = yearMonths;
        this.currentYearMonth = yearMonths[0];
      })
      .catch(err => {
        console.log(err);
      });
  }
};
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
h3 {
  margin: 40px 0 0;
}
ul {
  list-style-type: none;
  padding: 0;
}
li {
  display: inline-block;
  margin: 0 10px;
}
a {
  color: #42b983;
}
</style>
